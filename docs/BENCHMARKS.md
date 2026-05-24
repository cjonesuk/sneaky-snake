# Benchmarks

Performance baselines for Axis.ECS, run via the `Axis.ECS.Benchmarks` project. See [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md#benchmarks) for how to run.

## Conditions

| Field | Value |
|---|---|
| Date | 2026-05-24 |
| Machine | AMD Ryzen 7 5800X, 16 logical / 8 physical cores |
| OS | EndeavourOS (Linux 6.19.14-arch1-1) |
| .NET | 10.0.4, RyuJIT AVX2 |
| BenchmarkDotNet | 0.14.0 |
| Build | Release, no debugger |

Re-run on the same hardware to compare. Refresh this file when the numbers move materially (>~10%) or when a new benchmark is added.

## Baselines

### ForEachAllocationBenchmark

10,000 entities with (Health, Healing). `Query<Health, Healing>.ForEach(...)`.

| Method                        | Mean     | Allocated |
|------------------------------ |---------:|----------:|
| Static_Lambda_NoCapture       | 6.46 us  | **0 B** |
| NonStatic_Lambda_CapturesThis | 6.08 us  | **0 B** |

### IterateVsForEachBenchmark

Same setup. `Query<Health, Healing>.Iterate(...)` (per-archetype span) vs `.ForEach(...)` (per-entity).

| Method             | Mean     | Allocated |
|------------------- |---------:|----------:|
| Iterate_SpanAccess | 4.34 us  | 0 B |
| ForEach_PerEntity  | 5.63 us  | 0 B |

### QueryCacheBenchmark

5 archetypes, 1,000 entities each (~1,000 matching). Pre-built `Query<Health>` vs rebuilt per call.

| Method             | Mean     | Gen0   | Allocated |
|------------------- |---------:|-------:|----------:|
| Warm_CachedQuery   | 1.36 us  | -      | 0 B |
| Cold_BuildEachCall | 1.21 us  | 0.0172 | 288 B |

## Findings

### .NET 10 escape analysis is doing what we hoped

Non-static lambda capturing `this` allocates **zero** bytes — same as static lambda. The Systems API design (no TState variants, no required `static`) is validated. The non-static version is even slightly faster (~6%), probably because the JIT can specialize a concrete closure type more aggressively than a generic static delegate. See [dotnet10-closure-policy memory](../../.claude/projects/-home-chris-dev-sneaky-snake/memory/dotnet10-closure-policy.md).

### `Iterate` is ~30% faster than `ForEach`

The per-entity wrap (creating an `Entity` struct, invoking the delegate per step) costs ~130ps per entity. Negligible for game logic at 10k entities (1.3μs per frame), meaningful for tight inner loops. **Rule of thumb**: `ForEach` for ergonomics; `Iterate` when you're in the hot path and operating on spans.

### Query cache is a wash at this scale — needs a fairer test

Cold (rebuild + iterate) beats warm (cached + iterate) by 12% here. Almost certainly because iteration cost dwarfs the archetype scan when there are only 5 archetypes. The 288 B/call allocation on cold is real but small. The cache should win at higher archetype counts or higher per-frame call frequency — see backlog below.

---

## Benchmark backlog

Things worth measuring that aren't covered yet, roughly by priority.

### High-value next benchmarks

- **Query cache at scale**: same shape as `QueryCacheBenchmark` but with 50+ archetypes and only a few matching. Should make the cache win visible.
- **Entity creation throughput**: `world.DefineEntity().With(...).Build()` for 1/2/4/8 component arities. PingPong spawns dozens of entities per game start; need to know the floor.
- **Deferred command flush cost**: a batch of 100/1k/10k queued commands flushed at scope exit. The `WorldCommandQueue` + `BufferedEntityCreationCommand` paths haven't been profiled.
- **Component add/remove (archetype migration)**: `entity.Add<T>()` and `entity.Remove<T>()` trigger `MigrateEntityUp`/`MigrateEntityDown` which copy all columns. Measure 1, 5, 10 existing components.
- **Multi-arity scaling**: `Query<T0>` through `Query<T0..T7>.ForEach`. Does each extra component add linear cost, or does column lookup overhead dominate?
- **Event stream throughput**: per-entity vs world-level event emission + consumption. PingPong leans on these for collision/input.

### Medium-value

- **World cold start**: `World.Create()` plus first frame setup. PingPong's `Activate` does a lot here.
- **System scheduler overhead**: 10/100/1000 registered systems, each iterating 0 entities. How much pure dispatch cost?
- **Real-world scenario**: a benchmark that mimics PingPong's system mix (5 systems, ~10 entities) running for 1000 simulated frames. Catches integration regressions a microbenchmark would miss.

### Low-value / curiosity

- **Static vs non-static lambda again**, with capture chains of different depths (capture `this`, capture local, capture two locals, capture from outer method). The headline result already says it doesn't matter, but the boundaries are worth knowing.
- **AOT/NativeAOT**: compile the benchmarks AOT and compare. Would also flush out any reflection assumptions.

---

## Review / improvement backlog

Code areas worth looking at, ordered roughly by expected impact. Should be driven by benchmark data, not by speculation.

### Almost-certainly-worth-doing

- **`Archetype._componentColumns` is `IComponentValues[]`** — iteration boxes through the interface. Plan: replace with a concrete array indexed by column type, or generate per-arity archetype types. Big architectural change; needs benchmark justification.
- **`Archetype._componentIdToColumnIndex` is `Dictionary<Id, int>`** — for typical archetypes with <16 components, a linear scan over an `Id[]` would likely beat the dictionary on cache and constant-factor. Measurable change with low blast radius.
- **`QueryBuilder.Build` allocates `_terms.ToArray()`** — every cold query call pays this (see Cold_BuildEachCall's 288 B). Cache by terms-fingerprint, or pool builders.
- **`ArchetypeQuery._cachedResults` is `List<Archetype>`** with no automatic invalidation — `RegisterQuery` is defined ([World.cs:62](../src/Axis.ECS/World.cs#L62)) but never called. Need either (a) auto-register on `ArchetypeQuery` construction so cache invalidates on archetype churn, or (b) accept and document that queries are eternally-cached (current behavior).
- **`ComponentValues<T>` is `List<T>`** — already flagged in the root [README.md](../README.md#ecs-todo) as a target for fixed-size arrays + `Span<T>`. The `CollectionsMarshal.AsSpan` trick gets us span access but allocations on growth are still List-shaped.

### Worth investigating

- **`RemoveEntity` linear cost** — currently iterates all component columns and shifts. Swap-and-pop with the last entry would be O(1). Already flagged in [README.md](../README.md#ecs-todo).
- **Per-entity event streams** — `EventManager` keeps one stream per entity-id × event-type. With many short-lived entities, this could fragment. Pooling and reuse?
- **System scheduler ordering** — currently registration-order. As real games grow, dependency-aware ordering or grouping will matter.
- **Parallel system execution** — zero parallelism today. Query results are read-only between systems (deferred commands handle structural changes), so per-archetype parallelism over a single system's iteration is a natural fit. Benchmarks first.

### Speculative / want-to-try

- **SIMD vector component iteration** — `Transform2d.Position` is `Vector2`, batched math could go through `Vector128<float>`. Probably wants a dedicated `SimdQuery<T>` overload or generator path.
- **NativeAOT publish path** — would surface reflection/dynamic-codegen assumptions in source generators and PingPong. Useful for distribution and startup time.
- **Fixed-arity Query specializations** — e.g. hand-written `Query<T0>` that skips the array lookup for the single-component case. Microbenchmark question.

### Process improvements

- **Add a baseline-diff CI hook** — store last-known-good benchmark numbers and fail (or warn) on regression. Probably overkill until there's more than one benchmark run.
- **Markdown-snapshot the artifacts** — `BenchmarkDotNet.Artifacts/results/*-report-github.md` is auto-generated. Could be checked in (or referenced) as a versioned record. Tradeoff: noise in diffs.
- **Run benchmarks against PingPong scenarios** — extract PingPong's `SetupSystems` into a benchmark fixture for a realistic integration baseline.
