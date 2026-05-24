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
}
