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

        healthQuery.Run().ToList().Select(x => x.EntityType).ShouldBe(
        [
            entity1.GetEntityType(),
            entity2.GetEntityType()
        ]);

        healthAndHealingQuery.Run().ToList().Select(x => x.EntityType).ShouldBe(
        [
            entity2.GetEntityType()
        ]);

        healingQuery.Run().ToList().Select(x => x.EntityType).ShouldBe(
        [
            entity2.GetEntityType(),
            entity3.GetEntityType()
        ]);
    }
}