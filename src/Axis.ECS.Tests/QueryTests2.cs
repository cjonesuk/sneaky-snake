using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public static class TestExtensions
{
    public static List<Archetype> ToList(this ArchetypeEnumerable enumerable)
    {
        List<Archetype> archetypes = [.. enumerable];
        return archetypes;
    }
}

public class QueryTests2
{
    [Fact]
    public void Test()
    {
        var world = World.Create();

        var entity1 = world.DefineEntity()
            .With(new Health(100))
            .Build();

        var entity2 = world.DefineEntity()
            .With(new Health(100))
            .With(new Healing(10))
            .Build();

        var entity3 = world.DefineEntity()
            .With(new Healing(10))
            .Build();

        var healthQuery = QueryBuilder
           .For(world)
           .Add<Health>()
           .Build();

        var healthAndHealingQuery = QueryBuilder
            .For(world)
            .Add<Health>()
            .Add<Healing>()
            .Build();

        var healingQuery = QueryBuilder
            .For(world)
            .Add<Healing>()
            .Build();

        healthQuery.Run().ToArray().Select(x => x.EntityType).ShouldBe(
        [
            entity1.GetEntityType(),
            entity2.GetEntityType()
        ]);

        healthAndHealingQuery.Run().ToArray().Select(x => x.EntityType).ShouldBe(
        [
            entity2.GetEntityType()
        ]);

        healingQuery.Run().ToArray().Select(x => x.EntityType).ShouldBe(
        [
            entity2.GetEntityType(),
            entity3.GetEntityType()
        ]);
    }
}

public class QueryT0Tests
{
    private readonly World _world;

    public QueryT0Tests()
    {
        _world = World.Create();

        _world.DefineEntity()
           .With(new Health(100))
           .Build();

        _world.DefineEntity()
           .With(new Health(90))
           .With(new Healing(10))
           .Build();
    }

    [Fact]
    public void Iterate()
    {
        var healthQuery = DefineQuery.For<Health>(_world)
           .Build();

        int healthTotal = 0;
        healthQuery.Iterate((ids, health) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healthTotal += health[i].Value;
            }
        });
        healthTotal.ShouldBe(190);
    }

    [Fact]
    public void ForEach()
    {
        var healthQuery = DefineQuery.For<Health>(_world)
           .Build();

        int healthTotal = 0;
        healthQuery.ForEach((Entity entity, ref Health health) =>
        {
            healthTotal += health.Value;
        });
        healthTotal.ShouldBe(190);
    }
}

public class QueryT1Tests
{
    private readonly World _world;

    public QueryT1Tests()
    {
        _world = World.Create();

        _world.DefineEntity()
           .With(new Health(100))
           .With(new Healing(10))
           .Build();

        _world.DefineEntity()
           .With(new Health(90))
           .With(new Healing(20))
           .Build();
    }

    [Fact]
    public void Iterate()
    {
        var query = DefineQuery.For<Health, Healing>(_world)
           .Build();

        int healthTotal = 0;
        int healingTotal = 0;
        query.Iterate((ids, health, healing) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healthTotal += health[i].Value;
                healingTotal += healing[i].Amount;
            }
        });
        healthTotal.ShouldBe(190);
        healingTotal.ShouldBe(30);
    }

    [Fact]
    public void ForEach()
    {
        var query = DefineQuery.For<Health, Healing>(_world)
           .Build();

        int healthTotal = 0;
        int healingTotal = 0;
        query.ForEach((Entity entity, ref Health health, ref Healing healing) =>
        {
            healthTotal += health.Value;
            healingTotal += healing.Amount;
        });
        healthTotal.ShouldBe(190);
        healingTotal.ShouldBe(30);
    }
}