# Branch Status — `components-as-entities`

Snapshot taken 2026-05-22. Last commit on this branch: 2026-02-26 (~3 months idle).

## What this branch is

A substantial refactor of `Axis.ECS`. Three threads of work:

1. **Components as entities** — `ComponentEntityManager` + `IdSpace` map component-type IDs into the same ID space as regular entities. Sets up the future for component metadata, relationships (pairs), and IsA tagging.
2. **Query system rewrite** — Removed the old `world.QueryEach(...)` API in favor of a source-generated `Query<T0..Tn>` family + fluent [QueryBuilder](../src/Axis.ECS/Queries/QueryBuilder.cs). Higher-level `world.System<T...>().ForEach(...)` extension wraps it. Source generator lives at [src/Axis.ECS.Generators/QueryGenerator.cs](../src/Axis.ECS.Generators/QueryGenerator.cs).
3. **EntityBuilder pattern** — Fluent entity creation backed by `BufferredEntityCreationCommand` and `FastBuffer`, replacing the older `AddEntityCommand` path.

16 commits, ~2.4k lines added / ~2.3k removed.

## Current state

- **Architecturally ~90% there.** The new query API is generated, tested, and consumed correctly by `PingPong`'s `SetupSystems()`.
- **Build is broken.** Signature drift between [IWorldSystem](../src/Axis.ECS/Systems/IWorldSystem.cs) and two implementers (see below).
- **One uncommitted change** in `IWorldSystem.cs`: deletes a ~32-line commented-out `Iter` ref-struct. Pure cleanup, harmless to commit.
- **5 old test files disabled** in `src/Axis.ECS.Tests` (`.disabled` extension): `SystemTests`, `WorldTests`, original `QueryTests`, etc. They exercised APIs that were intentionally redesigned. New tests added: `QueryTests2`, `EntityBuilderTests`. Coverage shrank in absolute terms.

## What's broken — signature drift

| File | Signature | Matches interface? |
|---|---|---|
| [IWorldSystem.cs:7](../src/Axis.ECS/Systems/IWorldSystem.cs#L7) | `void Execute(WorldSystemContext data);` | (definition) |
| [DelegatedWorldSystem](../src/Axis.ECS/Systems/IWorldSystem.cs#L24) | `void Execute(WorldSystemContext context)` | yes |
| [CollisionSystem](../src/Axis.Engine/Collision/CollisionSystem.cs#L18) | `void Execute(ref WorldSystemContext data)` | **no** |
| [PlayGameMode](../games/PingPong/PlayGame/PlayGameMode.cs#L258) | `void IWorldSystem.Execute(ref WorldSystemContext data)` | **no** |

`dotnet build` halts at `Axis.Engine` (CollisionSystem) before reaching PingPong, so only one error appears in the output — but both must be fixed.

Beyond the signature, `CollisionSystem` body is stale: it still calls `world.QueryEach(ref refthis, static (ref CollisionSystem sys, ref Iter iter, ref Transform2d transform, ref CollisionBody body) => ...)`. `QueryEach` and the old `Iter` ref-struct were removed in this branch. Even after the signature is fixed, the body won't compile — it needs a full migration to the new `world.System<T...>().ForEach(...)` pattern.

## Recovery plan

Goal: branch builds, all tests pass, `PingPong` runs.

### Step 1 — Pick a `WorldSystem.Execute` signature

`WorldSystemContext` is a `sealed class` ([WorldSystemContext.cs](../src/Axis.ECS/Systems/WorldSystemContext.cs)). Passing a class by `ref` provides no real perf benefit, so the simpler fix is to remove `ref` from the two consumer implementations. But both consumers were written with `ref`, suggesting the intent may be to make `WorldSystemContext` a ref struct later.

- **Option A (unblocking, recommended):** keep interface as `Execute(WorldSystemContext)`, remove `ref` from CollisionSystem and PlayGameMode.
- **Option B (preserves consumer intent):** change interface and `DelegatedWorldSystem` to `Execute(ref WorldSystemContext)`. Defer until WorldSystemContext is actually converted to a ref struct.

### Step 2 — Migrate `CollisionSystem` to the new query API

Model: PingPong's `SetupSystems()` ([PlayGameMode.cs:118-236](../games/PingPong/PlayGame/PlayGameMode.cs#L118-L236)). Specifically the `Transform2d + Paddle + CollisionBody` system at line 159 is the closest match in shape.

Concrete shape:

```csharp
world.System<Transform2d, CollisionBody>()
    .ForEach(static (ref context, ref iter, ref transform, ref body) =>
    {
        // iter.Id replaces the old iter.Id from the deleted Iter struct
        // route results out via an event stream rather than instance fields
    });
```

Sub-issue: `_currentCollisions` / `_previousCollisions` are instance fields. The new query lambdas are `static` and capture nothing. Two options:
1. Switch to non-static lambdas and capture `this` (simpler, hotter).
2. Push detections into an event stream and consume in a follow-up system (more idiomatic, avoids capture).

Files to modify:
- [src/Axis.Engine/Collision/CollisionSystem.cs](../src/Axis.Engine/Collision/CollisionSystem.cs)
- Possibly [src/Axis.ECS/Systems/IWorldSystem.cs](../src/Axis.ECS/Systems/IWorldSystem.cs) + [games/PingPong/PlayGame/PlayGameMode.cs](../games/PingPong/PlayGame/PlayGameMode.cs) — depends on Step 1

### Step 3 — Commit the IWorldSystem.cs cleanup

The uncommitted deletion of the commented `Iter` block is harmless cleanup. Fold it into the same commit as Step 1.

### Step 4 — Disabled tests: note only

Do not restore or delete the 5 `.disabled` test files this round. They exercised APIs that were intentionally redesigned. Surface them in the followup list.

## Verification

After Steps 1–3:

1. `dotnet build` from repo root — zero errors. Capture warnings.
2. `dotnet test` from repo root — all active tests pass.
3. `cd games/PingPong && dotnet run` — game window opens, paddles respond, ball bounces, scoring works. Visual confirmation; UI is not test-covered.
4. `git diff main...HEAD --stat` — sanity-check the branch is still a coherent refactor.

Once green: merge to `main`, open a PR, or park intentionally.

## Followups (not this round)

- Decide whether to restore/port the 5 disabled test files or delete them.
- `Axis.Collision` project is a near-empty stub. Fold into `Axis.Engine/Collision/` or build out before the next game.
- Empty placeholder `SystemBuilder` class in [IWorldSystem.cs:50](../src/Axis.ECS/Systems/IWorldSystem.cs#L50).
