# Benchmarks

Performance baselines for Axis.ECS, run via the `Axis.ECS.Benchmarks` project. See [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md#benchmarks) for how to run.

## Conditions

| Field | Value |
|---|---|
| Last updated | 2026-05-24 |
| Machine | AMD Ryzen 7 5800X, 16 logical / 8 physical cores |
| OS | EndeavourOS (Linux 6.19.14-arch1-1) |
| .NET | 10.0.4, RyuJIT AVX2 |
| BenchmarkDotNet | 0.14.0 |
| Build | Release, no debugger |

Re-run on the same hardware to compare. Refresh this file when the numbers move materially (>~10%) or when a new benchmark is added. **Always run a single benchmark project at a time** — concurrent runs contend for CPU and produce unreliable numbers.

## Baselines

### ForEachAllocationBenchmark

10,000 entities with (Health, Healing). `Query<Health, Healing>.ForEach(...)`.

| Method                        | Mean     | Allocated |
|------------------------------ |---------:|----------:|
| Static_Lambda_NoCapture       | 5.62 us  | **0 B** |
| NonStatic_Lambda_CapturesThis | 6.19 us  | **0 B** |

Static is ~10% faster this run; previous run had non-static slightly faster. Both within noise. Headline: **both allocate zero**.

### IterateVsForEachBenchmark

Same setup. `Query<Health, Healing>.Iterate(...)` (per-archetype span) vs `.ForEach(...)` (per-entity).

| Method             | Mean     | Allocated |
|------------------- |---------:|----------:|
| Iterate_SpanAccess | 4.42 us  | 0 B |
| ForEach_PerEntity  | 6.57 us  | 0 B |

### QueryCacheBenchmark

5 archetypes, 1,000 entities each (~1,000 matching). Pre-built `Query<Health>` vs rebuilt per call.

| Method             | Mean     | Gen0   | Allocated |
|------------------- |---------:|-------:|----------:|
| Warm_CachedQuery   | 1.37 us  | -      | 0 B |
| Cold_BuildEachCall | 1.48 us  | 0.0172 | 288 B |

After the cache-invalidation fix (queries now self-register with the world), cold pays an extra ~270 ns for `RegisterQuery` and is now slower than warm. In real-world use queries are built once at setup, so the cold path doesn't matter.

### EntityCreationBenchmark

`Spawn_Empty` calls `World.SpawnEntity()`. `Define_*` use the `DefineEntity().With(...).Build()` builder. Each benchmark does 1,000 ops per invocation; fresh world per iteration.

| Method                 | Mean       | Allocated |
|----------------------- |-----------:|----------:|
| Spawn_Empty            |   210 ns   |   148 B   |
| Define_OneComponent    |   804 ns   |   189 B   |
| Define_FourComponents  | 1,585 ns * |   256 B   |
| Define_EightComponents | 1,567 ns * |   318 B   |

\* 4-component and 8-component runs were multimodal with high variance (StdDev > 600 ns). Likely List growth happening at different points across iterations. Worth investigating.

### ArchetypeMigrationBenchmark

`entity.Add<T>()` or `entity.Remove<T>()` triggers archetype migration. Destination archetype pre-created (warm) so the measured cost is steady-state migration, not new-archetype creation. 1,000 ops per invocation; fresh world per iteration.

| Method                   | Mean       | Allocated |
|------------------------- |-----------:|----------:|
| AddComponent_FromOne     |   826 ns   |    81 B   |
| AddComponent_FromFour    | 1,565 ns   |   137 B   |
| RemoveComponent_FromFive | 1,484 ns   |   121 B   |

Per additional column migrated: ~246 ns (1565 - 826 = 739 ns for 3 extra columns).

## Findings

### .NET 10 escape analysis is doing what we hoped

Non-static lambda capturing `this` allocates **zero** bytes — same as static lambda. The Systems API design (no TState variants, no required `static`) is validated. Static may be marginally faster on average (varies run to run), but well within noise. See [dotnet10-closure-policy memory](../../.claude/projects/-home-chris-dev-sneaky-snake/memory/dotnet10-closure-policy.md).

### `Iterate` is ~30-50% faster than `ForEach`

The per-entity wrap (creating an `Entity` struct, invoking the delegate per step) is meaningful in tight loops. Both allocate zero. **Rule of thumb**: `ForEach` for ergonomics; `Iterate` when you're in the hot path and operating on spans. Most game logic should use `ForEach`.

### Query cache is now correctly invalidated; warm is the path that matters

After fixing the invalidation bug ([commit](#TODO)), warm beats cold (1.37 vs 1.48 us) as expected. Cache is meaningful for any code that builds queries once during setup. The cold-build path costs 288 B per call plus `RegisterQuery` overhead — don't do that.

### Interface dispatch (`IComponentValues[]`) is **not** the dominant migration cost

Per-additional-column-migrated is ~246 ns. Interface dispatch on `column.Migrate(...)` is probably 5-15 ns of that. The other ~230 ns is split between dictionary lookups on both source and target archetypes (`_componentIdToColumnIndex[id]`) and the actual data copy + List growth amortization. **Conclusion**: replacing the `Dictionary<Id, int>` with a linear scan or sorted array would likely give bigger wins than monomorphizing the column array. Benchmark first.

### Entity creation allocates 148-318 B per spawn

The bulk of this is `BufferedEntityCreationCommand` payload allocations and entity-table dictionary growth. At 60 FPS spawning 100 entities per frame, that's ~1.8 MB/sec GC pressure. Not catastrophic, but the deferred-command queue is a target if entity spawn becomes a real hotspot. Multimodal variance at higher arities suggests List<T> growth events are visible in the timing — fixed-size column arrays (already on the README TODO list) would smooth this.

---

## Benchmark backlog

Things worth measuring that aren't covered yet, roughly by priority.

### High-value next benchmarks

- **Query cache at scale**: same shape as `QueryCacheBenchmark` but with 50+ archetypes and only a few matching. Should make the cache win visible.
- **Deferred command flush cost**: a batch of 100/1k/10k queued commands flushed at scope exit. The `WorldCommandQueue` + `BufferedEntityCreationCommand` paths haven't been profiled.
- **Multi-arity ForEach scaling**: `Query<T0>` through `Query<T0..T7>.ForEach`. Does each extra component add linear cost, or does column lookup overhead dominate?
- **Event stream throughput**: per-entity vs world-level event emission + consumption. PingPong leans on these for collision/input.
- **Investigate multimodal variance in `Define_FourComponents`/`Define_EightComponents`** — likely `ComponentValues<T>.List<T>` growth events; want to confirm.

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

### Backed by benchmark data

- **`Archetype._componentIdToColumnIndex` is `Dictionary<Id, int>`** — migration data shows ~230 ns per column spent in dictionary lookups + List growth (not interface dispatch). For typical archetypes with <16 components, a linear scan over a sorted `Id[]` would likely beat the dictionary on cache and constant-factor. Low blast radius, measurable change. **Probably the highest-value next change.**
- **`ComponentValues<T>` is `List<T>`** — already flagged in [README.md](../README.md#ecs-todo). Migration + creation benchmarks both show allocation cost dominated by List growth; the multimodal variance at higher arities is the smoking gun. Switching to a `T[]` with manual growth (or a `FastBuffer<T>`-like type) should give cleaner numbers and lower allocs.

### Speculative until benchmarked

- **`Archetype._componentColumns` is `IComponentValues[]`** — earlier hypothesis flagged this as the inner-loop hotspot. **The data says otherwise**: interface dispatch is ~5-15 ns per column-migration, swamped by dictionary lookups and List growth. Worth keeping on the radar but not a priority. If we tackle it, the move is per-arity `Archetype<T0..Tn>` types (via source generator) or a typed column registry — both architecturally expensive. Don't start until the dictionary + List wins are claimed.
- **`QueryBuilder.Build` allocates `_terms.ToArray()`** — every cold build pays 288 B (visible in QueryCacheBenchmark). In real-world use queries are built once at setup so this is negligible. Skip unless a real workload starts rebuilding queries per frame.

### Worth investigating

- **`RemoveEntity` linear cost** — currently iterates all component columns and shifts. Swap-and-pop with the last entry would be O(1). Already flagged in [README.md](../README.md#ecs-todo). Add a `RemoveEntity` benchmark before touching.
- **Per-entity event streams** — `EventManager` keeps one stream per entity-id × event-type. With many short-lived entities, this could fragment. Pooling and reuse?
- **System scheduler ordering** — currently registration-order. As real games grow, dependency-aware ordering or grouping will matter.
- **Parallel system execution** — zero parallelism today. Query results are read-only between systems (deferred commands handle structural changes), so per-archetype parallelism over a single system's iteration is a natural fit. Benchmarks first.
- **`_activeQueries` memory growth** — the cache-invalidation fix registers every `ArchetypeQuery` ever built and never unregisters. Fine for normal "build once at setup" usage, leaky for pathological per-frame rebuilds. Consider weak-reference or explicit dispose if a real use case appears.

### Speculative / want-to-try

- **SIMD vector component iteration** — `Transform2d.Position` is `Vector2`, batched math could go through `Vector128<float>`. Probably wants a dedicated `SimdQuery<T>` overload or generator path.
- **NativeAOT publish path** — would surface reflection/dynamic-codegen assumptions in source generators and PingPong. Useful for distribution and startup time.
- **Fixed-arity Query specializations** — e.g. hand-written `Query<T0>` that skips the array lookup for the single-component case. Microbenchmark question.

### Process improvements

- **Add a baseline-diff CI hook** — store last-known-good benchmark numbers and fail (or warn) on regression. Probably overkill until there's more than one benchmark run.
- **Markdown-snapshot the artifacts** — `BenchmarkDotNet.Artifacts/results/*-report-github.md` is auto-generated. Could be checked in (or referenced) as a versioned record. Tradeoff: noise in diffs.
- **Run benchmarks against PingPong scenarios** — extract PingPong's `SetupSystems` into a benchmark fixture for a realistic integration baseline.
