# Project Overview

A hobby C# game engine and game built around an archetype-based Entity-Component-System (ECS), targeting `net10.0`. The repo name (`sneaky-snake`) is older than the current game — the implemented game is `PingPong`.

The stated goal is learning + performance experimentation in C#.

## Layout

```
sneaky-snake.sln
├── src/
│   ├── Axis.Core          utility library (collections, native memory, FastBuffer)
│   ├── Axis.ECS           core ECS framework
│   ├── Axis.ECS.Generators Roslyn source generator (typed Query overloads)
│   ├── Axis.Engine        engine layer (rendering, input, collision)
│   └── Axis.Collision     stub project — currently near-empty
├── games/
│   └── PingPong           the one game; complete and playable when build is green
├── README.md              high-level intent + TODOs
└── Axis.md                framework planning notes
```

Test projects mirror the source structure: `Axis.ECS.Tests`, `Axis.Core.Tests`, plus placeholders for engine/collision. xUnit v3 + Shouldly.

## The Axis.ECS framework

This is the heart of the project.

### Core model

- **Entity** — an `Id` (struct wrapping an int with a generational/space mask). Has no data of its own.
- **Component** — an `unmanaged` struct attached to an entity. Identified by a `ComponentTypeId`.
- **Archetype** — the storage bucket for all entities sharing the same set of component types. Per-archetype, each component type is held in its own contiguous column (`ComponentValues<T>`). This is the SoA (struct-of-arrays) layout that makes batch iteration fast.
- **EntityType** — a value describing which component IDs make up an archetype; used as the archetype lookup key.
- **World** — owns the `ArchetypeManager`, `ComponentEntityManager`, entity-ID allocator, system scheduler, event manager, and a deferred-command queue.

### Components as entities (the current refactor)

`ComponentEntityManager` + `IdSpace` assign every registered component type its own `Id` drawn from the same space as regular entities. This sets up:
- Component metadata stored as components (reflective ECS).
- Relationship pairs (e.g. `IsA`, `ChildOf`) encoded as `Id.Pair(relationshipId, targetId)`.
- Tag entities that aren't components.

This is in-progress on the `components-as-entities` branch. See [BRANCH-STATUS.md](BRANCH-STATUS.md).

### Queries and systems

Queries are constructed via `QueryBuilder.For(world).Add<T1>().Add<T2>().Build()` and return an `ArchetypeQuery` that caches matched archetypes until the world structure changes (then `Invalidate` is called).

On top of that sits a source-generated typed family of `Query<T0..T7>` (up to 8 component types). Two shapes: `ForEach((Entity entity, ref T0, ...) => ...)` for per-entity iteration, and `Iterate((Span<Id> ids, Span<T0> c0, ...) => ...)` for per-archetype span access. The generator is in `Axis.ECS.Generators`. The wrapper `world.System<T...>().ForEach(...)` registers an `IWorldSystem` that runs the body each frame.

Systems implement `IWorldSystem` and are registered with `world.AddSystem(...)`. `ExecuteSystems(deltaTime)` runs them in registration order, each within a deferred-commands scope so structural changes (entity creation, component add/remove) don't invalidate iteration mid-frame.

### Deferred commands

`World.BeginDeferringCommands()` returns a `WorldDeferredCommandsScope` (disposable). While open, structural ops queue into `WorldCommandQueue` instead of executing immediately:
- `AddEntityCommand`, `RemoveEntityCommand`
- `AddComponentCommand`, `RemoveComponentFromEntityCommand`, `SetComponentCommand`
- `BufferedEntityCreationCommand` (used by `EntityBuilder`)
- `ClearAllEntitiesCommand`

On scope dispose, the queue is applied and cleared.

### Events

`EventManager` exposes per-type and per-entity `EventStream<T>` queues. Systems publish via `events.GetEventStream<T>(entityId?).AddEvent(...)` and consume via `AsSpan()`. PingPong uses this for input, collision, and goal events.

## The Engine layer

`Axis.Engine` is the substrate above the pure ECS:

- **GameEngine** — frame loop, world ownership, device management.
- **Rendering** — render passes, render commands, world renderer driven off a camera entity. Per Axis.md, the design is per-thread render command lists merged into per-layer (background/scene/ui/debug) passes sorted by z-index. The current implementation is well short of that — it's a basic single-pass setup.
- **Input** — device abstraction with action mapping (input context → action ID → event stream). Working enough for PingPong.
- **Collision** — `CollisionSystem` does brute-force pairwise checks for circles and AABBs, emits `CollisionStartedEvent` / `CollisionEvent` / `CollisionEndedEvent` plus per-entity `CollisionWithEvent` streams. Currently broken (see branch status).

## The game

PingPong is a complete playable game: two paddles, ball physics, goal detection, scoring to a max score, end-game flow. Lives in `games/PingPong/`. Its `PlayGameMode.cs` is the closest thing to a "how do I actually use this framework" reference.

## Benchmarks

`src/Axis.ECS.Benchmarks/` is a BenchmarkDotNet project measuring the ECS hot paths. Run with:

```bash
dotnet run -c Release --project src/Axis.ECS.Benchmarks -- --filter '*'
```

Filter to a single class with `--filter '*ForEachAllocationBenchmark*'`, or smoke-test the infrastructure cheaply with `--job dry`. Three baselines today: ForEach allocation (validates the .NET 10 escape-analysis assumption — static and non-static lambdas should both report 0 B/op), Iterate-vs-ForEach throughput, and query cache hit cost. See [BENCHMARKS.md](BENCHMARKS.md) for the current numbers, findings, and the benchmark/review backlog.

---

# Quality Assessment

Honest, in the spirit you asked. Hobby/learning context — these are observations to inform what to work on, not criticism.

## Overall rating

**6.5–7 / 10 for a hobby learning project.** Strong architectural instincts, mid-state implementation, in-progress refactor that's improving the design but isn't done. The thing builds and runs (on `main`); just not on this branch right now.

### Strengths

- **Right architecture.** Archetype + SoA is the correct pattern for what's being built. Decision to use it (rather than a sparse-set or naive object-per-entity layout) is well above the typical hobby ECS bar.
- **Source-generated query overloads.** Avoiding reflection and per-call generics is the right call, and the generator approach is clean.
- **Deferred-commands-as-scope.** `using var _ = world.BeginDeferringCommands();` is ergonomic and idiomatic C#. Makes the "don't mutate during iteration" rule fall out naturally.
- **Separation of concerns.** Core / ECS / Engine / Game layering is honest and PingPong stays on the right side of each line.
- **Span-based iteration.** Hot paths return `Span<T>` over component columns; no boxing or virtual dispatch in the inner loop.
- **Event streams per entity.** Decoupling collisions / inputs / domain events through typed streams is a nice design choice and PingPong uses it well.

### Bugs / sharp edges found

| Severity | File | Issue |
|---|---|---|
| **High** | [CollisionSystem.cs:18](../src/Axis.Engine/Collision/CollisionSystem.cs#L18) | Signature mismatch with `IWorldSystem` — branch doesn't build. |
| **High** | [PlayGameMode.cs:258](../games/PingPong/PlayGame/PlayGameMode.cs#L258) | Same signature mismatch (only masked because build halts earlier). |
| **High** | `CollisionSystem` body | Still calls removed `world.QueryEach` and the deleted `Iter` struct. Needs rewrite. |
| Medium | [World.cs:179](../src/Axis.ECS/World.cs#L179) | `RemoveAllEntities()` enqueues in deferred mode but **falls through** and also clears immediately. Should `return` after enqueue. |
| Medium | [World.cs:257](../src/Axis.ECS/World.cs#L257) | `RemoveComponent<T>(ref id)` takes `Id` (a tiny struct) by `ref` unnecessarily — inconsistent with sibling APIs and confusing at the call site. |
| Medium | `RegisterComponent` writes `Console.WriteLine(...)` on every registration ([World.cs:70](../src/Axis.ECS/World.cs#L70)). Debug spew leaks into PingPong startup. |
| Low | "Bufferred" spelled with two r's — `BufferredEntityCreationCommand`. |
| Low | Empty placeholder class `SystemBuilder` in [IWorldSystem.cs:50](../src/Axis.ECS/Systems/IWorldSystem.cs#L50). |
| Low | `Axis.Collision` project is a near-empty stub yet referenced and built. Cruft. |
| Low | `// todo: handle this scenario instead of failing` in `FindEntity` — silent dangerous-throw on lookup. |

### API design observations

**Good:**
- `world.System<T...>().ForEach(...)` reads cleanly.
- `EntityBuilder` chain is idiomatic.
- `WorldDeferredCommandsScope` and `using var _ = ...` is a delight to use.
- Static-factory entry points (`World.Create`, `WorldSystemContext.For`) are consistent in their corner of the API.

**Could be tightened:**
- Two ways to create entities (`CreateEntity` immediate vs `DefineEntity` builder). The names don't telegraph which is which — consider `world.SpawnNow()` vs `world.DefineEntity()`, or just unify around the builder.
- `SetComponentOnEntity` does double-duty as add-or-update. That's pragmatic but the name says "set." Consider `EnsureComponent` for the add-or-update case and a strict `SetComponent` that throws if absent.
- `internal` properties (`Components`, `Archetypes`, `Events`) leak across what should be public boundaries — needed for extension methods today, but it suggests a missing `IWorld` extensibility seam.
- `Iter` in the new query namespace is a thin int-iterator; the name suggests more than it does. Worth either expanding it (entity ID + component columns) or renaming to `IndexEnumerator`.
- The "explicit interface implementation with wrong signature" (`void IWorldSystem.Execute(ref ...)`) bug suggests the interface contract isn't being enforced at the IDE level. Worth keeping non-explicit implementations until the signature is settled.

### Performance design vs reality

**Intent is good** — SoA columns, spans, source-generated typed queries, deferred commands, FastBuffer.

**Reality has gaps:**
- `Archetype` uses `Dictionary<Id, int>` + `List<>` for columns. Lookup-by-componentId is O(1) but boxes through the dictionary on every component access. A small linear scan over an `Id[]` would beat it for the typical archetype with <16 components.
- `_componentColumns` array is iterated as `foreach (var column in _componentColumns)` which boxes through `IComponentValues`. For tight inner loops this would be visible in a profile.
- `QueryBuilder.Build()` calls `_terms.ToArray()` — allocates per build. Cache queries, build once.
- Query invalidation walks a `List<IArchetypeQuery>` linearly. Fine for now; would want to scope invalidation to affected component sets eventually.
- No SIMD / AOT / unsafe-pointer hot paths yet. The architecture supports them; nothing has needed them yet.
- **No benchmarks.** BenchmarkDotNet would be the natural next step before optimizing — measure before tightening.

### Test coverage

`Axis.ECS.Tests` has 8 active test files (post-refactor). Coverage is mostly unit tests of individual surfaces (entity creation, component management, query mechanics, ID masking, builder, archetype enumeration). The 5 `.disabled` integration tests covered cross-component flows that are now gone — coverage in that dimension is currently weak.

`Axis.Engine.Tests` and `Axis.Collision.Tests` are placeholders. Worth at least one smoke test apiece.

### Documentation

[README.md](../README.md) and [Axis.md](../Axis.md) exist but are mostly TODO lists and definitions. They've drifted from current reality (still mention "snake game", `cd cs && dotnet run`). Worth a refresh once this branch lands.

## What's next, roughly

In likely order of value:

1. Get this branch building and green (see [BRANCH-STATUS.md](BRANCH-STATUS.md)).
2. Decide what to do with the 5 disabled integration tests.
3. Spend one session on the obvious low-effort cleanups: `RemoveAllEntities` fall-through, `Console.WriteLine` removal, "Bufferred" → "Buffered" rename, `SystemBuilder` empty stub, `Axis.Collision` decision.
4. Add a BenchmarkDotNet project + a couple of benchmarks (1 archetype, N archetypes, query of 2/3 components). Measure baseline.
5. Once baseline exists, the column-storage micro-perf (replace Dictionary lookups with array scans) becomes a real decision instead of a guess.
6. Refresh README/Axis.md to match reality, fold in the design ideas that proved out.
