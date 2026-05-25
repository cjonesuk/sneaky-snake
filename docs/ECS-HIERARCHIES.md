# Hierarchies in Axis.ECS

Parent-child relationships between entities. Useful for any composed object — vehicle parts, equipped items, UI nesting, attached effects.

This doc covers (1) the API shipped today, (2) the conceptual relationship between ECS hierarchies and traditional 3D scene graphs, and (3) how other ECS engines model the same problem.

## The API

A child has a `Parent` component pointing at its parent's `Id`.

```csharp
public struct Parent
{
    public Id Value;
}
```

Set, get, remove, query via extensions on `Entity` / `IWorld`:

```csharp
child.SetParent(parent);
child.RemoveParent();
bool has = child.HasParent();
bool live = child.TryGetParent(out Entity parent);  // false if no Parent or parent dead
Entity p = child.GetParent();                        // throws if no live parent

// Zero-allocation enumeration. The query is built once at world construction
// and reused; foreach over a ref-struct enumerator does no heap work.
foreach (Entity child in world.GetChildren(parentId)) { ... }

world.RemoveEntityRecursive(parentId);               // depth-first cascade
```

Plain `RemoveEntity` does **not** cascade — children become orphans (their `Parent.Value` points at a dead Id; `TryGetParent` returns false). Use `RemoveEntityRecursive` to remove a subtree explicitly. The asymmetry is on purpose: silent cascade is a footgun when entities have mixed lifetimes.

No automatic `Children` list is maintained on the parent. `GetChildren` walks the cached `Parent` archetype-query and filters by parent Id. Cost is O(entities-with-any-Parent). Acceptable for typical hierarchies (tens of children, hundreds total); will get slow if every entity in a 10k-entity world has a parent. A `Children` component cached automatically becomes a one-system change once hooks/observers land — at that point `GetChildren` becomes O(children-of-this-parent).

## ECS hierarchies vs traditional 3D scene graphs

These model the same thing — a tree of relationships — but with different mechanism.

| | Traditional 3D engine | ECS |
|---|---|---|
| What the tree is | Tree of node pointers (`Parent*`, `Children[]`) | Rows in a component table |
| Where transforms live | On the node itself | Separate `LocalTransform` / `WorldTransform` components |
| How it's iterated | Recursive tree walk | Flat query (`Query<Parent>`); ordering enforced by systems if needed |
| Rendering iteration | Often the same walk as transform propagation | Separate query; renderer doesn't see the hierarchy |
| Culling | Often per-node, walking the tree | Separate spatial structure (BVH/quadtree/etc.) maintained by a system |

The "tree" in ECS exists only as data. You never construct it as a structure. Systems that need a tree-shaped traversal (e.g. propagating transforms parent-before-child) do that work at iteration time.

### Mesh attachment (player → helmet, sword)

In a traditional engine: the helmet node is a child of the player's hand bone; its world transform is computed from the bone's. In ECS: the helmet entity has a `Parent` component pointing at the player (or a specific bone entity), plus its own `Mesh` component, plus a `LocalTransform`. A transform-propagation system walks parents and writes the world transform.

### Vehicle with movable turret

Same shape. Tank body entity is the root. Turret entity has `Parent` = body, plus its own `LocalTransform` (the swivel offset). Barrel entity has `Parent` = turret, plus its own `LocalTransform`. Three entities, three `Parent` components, three transforms.

### Is ECS data separate from 3D data?

No. The 3D data **is** ECS data. Mesh handles, material handles, animation state, transforms — all components. The actual GPU resources (vertex buffers, textures) live outside ECS, referenced by handle/Id from components. The renderer is a system that queries entities with `Mesh + WorldTransform + ...` and emits draw calls.

### How do positions get updated?

A **transform propagation system**. Pseudocode:

```csharp
// Two-pass approach: roots first, then propagate down.
world.System<LocalTransform>().Without<Parent>().ForEach((Entity e, ref LocalTransform local) =>
{
    world.SetData(...);  // root's WorldTransform = its LocalTransform
});

// Then propagate to children, depth-first or in topological order.
// Implementation depends on whether you maintain a Children list, traverse via GetChildren, or sort by depth.
```

The naive approach iterates entities with `Parent` and reads the parent's already-computed `WorldTransform`. Requires either:
1. **Two-pass with sort by depth** — first compute depth for each entity, then iterate in depth order.
2. **Memoization on parent's state** — iterate `Parent`+`LocalTransform`, walk up to find a parent with up-to-date `WorldTransform`, cache as you go. Slower in pathological cases but simpler.
3. **Maintained children list** — visit roots, recurse via children. Cleanest, needs the auto-maintained `Children` (deferred until hooks).

**None of the above is implemented in Axis.ECS today.** This doc describes what would be built next; the hierarchy API alone gives you the foundation (parent/child relationships and a way to traverse them).

### What about scene graph / culling / spatial indexing?

Traditional scene graphs do two jobs in one tree: transform propagation AND rendering iteration order. ECS splits these:

- **Transform propagation** = one system, as above.
- **Rendering iteration** = a query over entities with renderable components. The hierarchy isn't walked.
- **Culling** = a third concern. Usually a separate spatial structure (BVH, quadtree, loose grid) maintained by a system that watches `WorldTransform` changes. The renderer queries this structure to get visible entities, not the hierarchy.

That spatial structure could be world-level data (via `world.SetData<SpatialIndex>(...)`) once a renderer that needs it exists. None of that is part of this round.

## How other engines model hierarchies

| Engine | Parent storage | Children storage | Cascade on delete |
|---|---|---|---|
| **flecs** | `(ChildOf, parent)` pair encoded on the child | implicit; queried via pair filter | yes, by default |
| **Unity DOTS** | `Parent { Entity Value; }` component | `DynamicBuffer<Child>` per-entity, auto-maintained by `TransformSystemGroup` | manual; cleanup buffers track `PreviousParent` |
| **Bevy** | `Parent(Entity)` tuple-struct component | `Children(SmallVec<[Entity; 8]>)` component auto-maintained by hooks | opt-in via `despawn_recursive` |
| **EnTT** | not built-in | not built-in | n/a |
| **Axis.ECS** (today) | `Parent { Id Value; }` component | none; `GetChildren` returns a ref-struct enumerator over the cached Parent query | opt-in via `RemoveEntityRecursive` |

We're closest to Bevy minus the auto-maintained `Children`. The maintained-list version becomes a one-system change once hooks/observers exist.

### Why not the flecs-style `(ChildOf, parent)` pair pattern?

flecs encodes the relationship as a pair id, with the parent's identity baked into the child's archetype signature. Children of the same parent cluster into one archetype with adjacent storage — fast "find all children of X", great cache locality, cascade-by-archetype-drop.

It's elegant *if* the underlying storage handles it well. In Axis.ECS today, every distinct `(ChildOf, parent)` pair-id would create a distinct archetype (since `World.SetPairOnEntity` treats the pair-id as a regular component-id). 1000 children of 1000 different parents = 1000 archetypes, each with one entity. Archetype creation invalidates active queries; this would thrash badly at scale.

flecs avoids the explosion with a sophisticated pair-storage layer (per-relationship indexing, target-as-secondary-key lookups). Replicating that is a multi-week storage redesign and a separate plan entirely. The component-based Parent shipping today doesn't preclude that direction — it's just a smaller bite.

## Followups (not in this round)

- **Transform propagation system** — `LocalTransform2d`, `WorldTransform2d`, a propagation system. Migrate PingPong's `Transform2d` to `LocalTransform2d`.
- **Automatic Children list** — once hooks/observers land, maintain a `Children` component on parents automatically. Replaces query-based `GetChildren` with O(children-count) iteration.
- **Pair-based relationships** — flecs-style `(ChildOf, parent)`, including the storage work to make it not explode.
- **Spatial indexing / culling** — separate plan when rendering grows beyond a single pass.

See [ECS-GAPS.md](ECS-GAPS.md) for the full backlog and engine comparison.
