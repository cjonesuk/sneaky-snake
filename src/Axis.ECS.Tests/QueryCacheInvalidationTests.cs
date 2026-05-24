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
    public void UnregisteredQuery_NoLongerInvalidates()
    {
        var world = World.Create();
        world.DefineEntity().With(new Health(100)).Build();

        var healthQuery = DefineQuery.For<Health>(world).Build();

        int count = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => count++);
        count.ShouldBe(1);

        healthQuery.Unregister();

        // Create a new matching archetype. Without invalidation the cache stays stale.
        world.DefineEntity().With(new Health(50)).With(new Healing(10)).Build();

        count = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => count++);
        count.ShouldBe(1, "unregistered query should return stale cached results");
    }

    [Fact]
    public void UnregisterAllQueries_StopsInvalidationForEveryRegisteredQuery()
    {
        var world = World.Create();
        world.DefineEntity().With(new Health(100)).Build();
        world.DefineEntity().With(new Healing(5)).Build();

        var healthQuery = DefineQuery.For<Health>(world).Build();
        var healingQuery = DefineQuery.For<Healing>(world).Build();

        // Prime both caches
        int hCount = 0, gCount = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => hCount++);
        healingQuery.ForEach((Entity entity, ref Healing g) => gCount++);
        hCount.ShouldBe(1);
        gCount.ShouldBe(1);

        world.UnregisterAllQueries();

        // Create new matching archetypes for both
        world.DefineEntity().With(new Health(10)).With(new Healing(2)).Build();
        world.DefineEntity().With(new Healing(7)).With(new Armor(1)).Build();

        hCount = 0; gCount = 0;
        healthQuery.ForEach((Entity entity, ref Health h) => hCount++);
        healingQuery.ForEach((Entity entity, ref Healing g) => gCount++);

        hCount.ShouldBe(1, "health query should be stale after unregister-all");
        gCount.ShouldBe(1, "healing query should be stale after unregister-all");
    }

    [Fact]
    public void Unregister_IsIdempotent()
    {
        var world = World.Create();
        world.DefineEntity().With(new Health(100)).Build();

        var query = DefineQuery.For<Health>(world).Build();
        query.Unregister();
        query.Unregister(); // second call should be a no-op, not throw
    }
}
