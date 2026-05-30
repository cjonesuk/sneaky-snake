using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public class QueryFilterTests
{
    [Fact]
    public void Without_ExcludesArchetypesContainingTheComponent()
    {
        var world = World.Create();

        var hOnly = world.DefineEntity().With(new Health(100)).Build();
        var hAndHealing = world.DefineEntity().With(new Health(50)).With(new Healing(5)).Build();

        var query = DefineQuery.For<Health>(world)
            .Without<Healing>()
            .Build();

        int count = 0;
        Id seenId = default;
        query.ForEach((Entity entity, ref Health h) =>
        {
            count++;
            seenId = entity.Id;
        });

        count.ShouldBe(1, "only the (Health) archetype should match; (Health, Healing) is excluded");
        seenId.ShouldBe(hOnly.Id);
    }

    [Fact]
    public void Without_AppliedToBuilderDirectly()
    {
        var world = World.Create();

        world.DefineEntity().With(new Health(10)).Build();
        world.DefineEntity().With(new Health(20)).With(new Armor(1)).Build();

        var query = RawQueryBuilder.For(world)
            .Add<Health>()
            .Without<Armor>()
            .Build();

        int sum = 0;
        foreach (var archetype in query.Run())
        {
            archetype.TryGetColumnSpan<Health>(out var span);
            for (int i = 0; i < span.Length; i++) sum += span[i].Value;
        }
        sum.ShouldBe(10);
    }

    [Fact]
    public void Without_NoMatchingArchetype_ReturnsEmpty()
    {
        var world = World.Create();

        world.DefineEntity().With(new Health(100)).With(new Healing(5)).Build();

        var query = DefineQuery.For<Health>(world)
            .Without<Healing>()
            .Build();

        int count = 0;
        query.ForEach((Entity entity, ref Health h) => count++);

        count.ShouldBe(0);
    }

    [Fact]
    public void Include_And_Exclude_AreInvalidatedTogetherWhenArchetypesChange()
    {
        var world = World.Create();

        var hOnly = world.DefineEntity().With(new Health(1)).Build();

        var query = DefineQuery.For<Health>(world).Without<Healing>().Build();

        int count = 0;
        query.ForEach((Entity entity, ref Health h) => count++);
        count.ShouldBe(1);

        // Add a new archetype that matches the include but is excluded by the without.
        world.DefineEntity().With(new Health(2)).With(new Healing(1)).Build();

        count = 0;
        query.ForEach((Entity entity, ref Health h) => count++);
        count.ShouldBe(1, "cache invalidates on new archetype; new (Health, Healing) archetype is excluded");
    }
}
