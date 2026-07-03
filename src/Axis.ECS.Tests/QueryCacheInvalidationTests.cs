using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public class QueryCacheInvalidationTests
{
    [Fact]
    public void CachedQuery_SeesArchetypeCreatedAfterFirstRun()
    {
        var world = World.Create();

        world.DefineEntity().With(new Health(100)).Build();

        var healthQuery = DefineQuery.For<Health>(world).Build();

        // Prime the cache with the (Health) archetype only
        int countBefore = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => countBefore++);
        countBefore.ShouldBe(1);

        // Create an entity with a different archetype: (Health, Healing).
        // This produces a NEW archetype that the cached query should pick up.
        world.DefineEntity().With(new Health(50)).With(new Healing(10)).Build();

        int countAfter = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => countAfter++);

        countAfter.ShouldBe(2, "query cache should invalidate when a new matching archetype is created");
    }

    [Fact]
    public void CachedQuery_StableWhenSameArchetypeReceivesMoreEntities()
    {
        var world = World.Create();

        world.DefineEntity().With(new Health(100)).Build();

        var healthQuery = DefineQuery.For<Health>(world).Build();

        int countBefore = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => countBefore++);
        countBefore.ShouldBe(1);

        // Add another entity to the SAME archetype (Health-only).
        // No new archetype, so cache stays valid by definition.
        world.DefineEntity().With(new Health(50)).Build();

        int countAfter = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => countAfter++);

        countAfter.ShouldBe(2);
    }

    [Fact]
    public void CachedQuery_SeesEntityAfterItMigratesToANewArchetype()
    {
        // Mirrors the render bug: a long-lived cached query (built once, reused every frame)
        // must still find an entity after Add<T>() migrates it into a brand-new archetype.
        // If invalidation fails, the entity's old archetype empties and the new one is absent
        // from the stale cache, so Count() drops to 0 -- i.e. the entity "disappears".
        var world = World.Create();

        var entity = world.SpawnEntity();
        entity.Set(new Health(100));

        var healthQuery = DefineQuery.For<Health>(world).Build();
        healthQuery.Count().ShouldBe(1); // prime the cache with the (Health) archetype

        // Gaining a component migrates the entity to a new (Health, Healing) archetype.
        entity.Add<Healing>();

        healthQuery.Count().ShouldBe(1, "cached query should still match the entity after it migrates to a new archetype");
    }
}
