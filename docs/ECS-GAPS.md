# ECS Feature Gaps

Comparison of Axis.ECS against established ECS engines, and which gaps are worth closing. Read this before planning the next feature; it'll save re-doing the comparison.

## Reference engines

- **[flecs](https://github.com/SanderMertens/flecs)** — C ECS with C++ wrapper. Famously feature-rich: relationships, prefabs, observers, hierarchies, modules, pipelines, REST inspector. The reference implementation for "comprehensive ECS."
- **[Unity DOTS Entities](https://docs.unity3d.com/Packages/com.unity.entities@latest)** — Unity's data-oriented C# ECS, with Burst (LLVM-based AOT) and the Job system for parallelism. Mature; production-used.
- **[Bevy ECS](https://bevyengine.org/)** — Rust game engine's ECS. Clean type-driven API. Resources, observers, change detection, system sets, run conditions.
- **[EnTT](https://github.com/skypjack/entt)** — C++ header-only ECS. Sparse-set storage (different from archetype). Heavy use of templates; popular in C++ game projects.

Axis.ECS is **archetype-based** like flecs / DOTS, not sparse-set like EnTT. Comparisons below assume archetype-style unless noted.

## What Axis.ECS has today

| Area | State |
|---|---|
| Entities | `Id` with 8-bit generation; world-allocated; alive checks. No recycling yet (see gap below). |
| Components | `unmanaged` structs only. Tag components (zero-size structs) work but aren't first-class. No shared components, no dynamic buffers. |
| Storage | Archetype-based; columns are typed `T[]`-backed `ComponentValues<T>`. Column lookup is a linear scan over the archetype's sorted `EntityType.ComponentIds` (post-dict-elimination). |
| Queries | Source-generated `Query<T0..T7>` (arities 1-8) with `ForEach((Entity, ref T0, ...))` and `Iterate((Span<Id>, Span<T0>, ...))`. Inclusion-only filters. |
| Systems | `IWorldSystem.Execute(ref WorldSystemContext)`; registration via `world.AddSystem` or the fluent `world.System<T...>().ForEach(...)`. Sequential, registration-order. |
| Deferred commands | `BeginDeferringCommands()` scope; commands for add/remove entity, add/remove/set component, batched entity creation via `EntityBuilder`. |
| Events | Typed event streams, global + per-entity. `world.Events.GetEventStream<T>()` / `(id)`. Consumed as `Span<T>`. |
| Relationships / pairs | `Id.Pair(rel, target)` encoding; `WorldPairs.IsA` built-in. No hierarchy convenience API, no wildcard pair queries. |
| Debug | `entity.DebugExport()` produces a snapshot. No system/query stats. |
| Components-as-entities | Foundation in place (`ComponentEntityManager`, ids share the entity-id space). Limited user-facing API yet. |

## Gap table

Effort: S (hours), M (1-2 days), L (week), XL (multi-week). Value: how often a real game needs this, not how cool it is.

| Gap | Axis.ECS | flecs | DOTS | Bevy | Effort | Value |
|---|---|---|---|---|---|---|
| **Query: exclude (`Without<T>`)** | missing | yes | `WithNone` | `Without<T>` | S | high |
| **Query: optional (`Optional<T>`)** | missing | yes | yes | yes | S | medium |
| **Query: OR (`AnyOf<T1, T2>`)** | missing | yes | `WithAny` | `Or<...>` | M | medium |
| **Query: wildcard pairs (`Likes(*)`)** | const defined, unused | yes | n/a | n/a | M | low (advanced) |
| **Hooks / observers (OnAdd/OnRemove/OnSet)** | missing | yes | partial (ISystemStartStop etc.) | observers | M | high |
| **Change detection (Changed<T>)** | missing (needs hooks first) | observer-based | yes | `Changed<T>` filter | L | medium |
| **Hierarchies (parent/child convenience)** | pair encoding exists, no API | built-in `ChildOf` | parent component | `Parent`/`Children` components | M | high |
| **Prefabs / cloning** | missing | yes | `EntityPrefab` | scenes / `Bundle` | M | high |
| **Bundles (reusable component sets)** | missing (EntityBuilder is per-call) | tags | bundles | `Bundle` trait | S-M | medium |
| **World data / singletons** | missing | `world.set<T>` | singleton components | `Resource<T>` | S | high |
| **Entity ID recycling** | broken (generations unused) | yes | yes | yes | S | high |
| **System dependencies / ordering** | registration-order only | yes (phases) | system groups | system sets | M | medium |
| **Conditional system execution** | missing | yes (entity filters) | enable/disable | `run_if` | S-M | low-medium |
| **Parallel iteration** | missing | yes (multi-threading mode) | jobs + Burst | yes | L-XL | medium (scale-dependent) |
| **Reflection / metadata** | foundation in `ComponentEntityManager`; no public API | yes | yes | yes | M | low (no concrete need yet) |
| **Serialization** | `ExportedEntity` is debug-only | yes | yes | yes | L | low (specialized) |
| **Dynamic per-entity buffers** (variable-length component data) | missing | n/a | `IBufferElementData` | n/a | M | low (specialized) |
| **Shared components** (one value across many entities) | missing | n/a | `ISharedComponentData` | n/a | M | low (specialized) |
| **REST / web inspector** | missing | yes | profiler integration | yes (3rd-party) | XL | low (hobby project) |
| **AOT / NativeAOT publish** | not tested | n/a | Burst (native codegen) | yes | M-L | low (specialized) |

## This round's scope (May 2026)

Three small high-value additions:

1. **`Without<T>` query filter** — Track 1 of the current plan. Smallest of the three; closes the obvious gap.
2. **World data API (`SetData<T>` / `GetData<T>` / `HasData<T>` / `RemoveData<T>`)** — Track 2. Bounded to **unmanaged** game-state structs (Score, GameState, etc.). Not for assets.
3. **Entity ID recycling** — Track 3. The 8-bit generation field on `Id` is currently unused; this makes it earn its existence.

Plan file: `/home/chris/.claude/plans/ok-its-been-quite-sparkling-donut.md`.

## Bigger features deferred (in rough priority order)

- **Hierarchies** — built on existing `Id.Pair`/`WorldPairs` foundation. `entity.AddChild`, `entity.Parent`, child enumeration. Common need for any game with composed objects.
- **Hooks / observers** — `world.OnAdd<T>`, `OnRemove<T>`, `OnSet<T>`. Architectural unlock for change detection and reactive systems later.
- **Prefabs / `entity.Clone()`** — once spawning N-of-a-template becomes a frequent pattern.
- **`Optional<T>` / `AnyOf<...>` query filters** — small follow-on to `Without<T>`.
- **System dependencies / pipelines** — when more than ~10 systems exist and ordering matters explicitly.
- **Change detection** — only after observers land.
- **Parallel iteration** — only when single-threaded perf becomes a real constraint. Not before.
- **`SystemRegistration<T...>.Without<T>()`** (the system-path version of Track 1's `Without`) — small, but requires deferring query construction until `ForEach`/`Iterate` is called.

## Explicitly not planned

These exist in other engines but aren't worth replicating in a hobby/learning ECS unless a concrete need appears:

- **Burst-style native codegen** — DOTS-specific, leans on LLVM. The JIT and source generators we have already give us most of what we'd need.
- **REST / web inspector** — flecs has it; it's huge surface area. A debug log overlay would be cheaper and more useful at this scale.
- **AOT / NativeAOT publish** — would surface assumptions in source generators and `Raylib_cs`. Useful for distribution; not urgent.
- **Shared components** (DOTS `ISharedComponentData`) — solves a real DOTS problem (chunk grouping by value) that we don't have.
- **Dynamic per-entity buffers** (DOTS `IBufferElementData`) — variable-length component data. Solvable today by storing an index into an external array; not worth the storage path.
- **Full serialization** — meaningful only when a save-game feature is on the table.

## Maintenance

Update this doc when:
- A gap closes (add a "✓ as of YYYY-MM-DD" note in the table row before deleting).
- A new gap is found (add a row).
- An engine's referenced feature changes meaningfully.

Last reviewed: 2026-05-25.
