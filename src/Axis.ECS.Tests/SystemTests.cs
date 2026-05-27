using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public class SystemTests
{
    [Fact]
    public void ForEach_WorksWithOneComponent()
    {
        var world = World.Create();
        world.DefineEntity().With(new Health(100)).With(new Healing(30)).Build();
        world.DefineEntity().With(new Health(50)).Build();

        int totalHealth = 0;
        int totalHealing = 0;

        world.System<Health>().ForEach((Entity entity, ref Health health) =>
        {
            totalHealth += health.Value;
        });

        world.System<Healing>().ForEach((Entity entity, ref Healing healing) =>
        {
            totalHealing += healing.Amount;
        });

        world.ExecuteSystems(10.0f);

        totalHealth.ShouldBe(150);
        totalHealing.ShouldBe(30);
    }

    [Fact]
    public void ForEach_WorksWithTwoComponents()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity().With(new Health(100)).With(new Healing(30)).Build();
        var entity2 = world.DefineEntity().With(new Health(50)).With(new Healing(20)).Build();
        var entity3 = world.DefineEntity().With(new Health(30)).Build();

        world.System<Health, Healing>().ForEach((Entity entity, ref Health health, ref Healing healing) =>
        {
            health.Value += healing.Amount;
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(130);
        entity2.Get<Health>().Value.ShouldBe(70);
        entity3.Get<Health>().Value.ShouldBe(30);
    }

    [Fact]
    public void ForEach_WorksWithThreeComponents()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity().With(new Health(100)).With(new Healing(30)).With(new Armor(10)).Build();
        var entity2 = world.DefineEntity().With(new Health(50)).With(new Healing(20)).With(new Armor(5)).Build();
        var entity3 = world.DefineEntity().With(new Health(30)).Build();

        world.System<Health, Healing, Armor>().ForEach((Entity entity, ref Health health, ref Healing healing, ref Armor armor) =>
        {
            health.Value += healing.Amount;
            armor.Value += 1;
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(130);
        entity1.Get<Armor>().Value.ShouldBe(11);
        entity2.Get<Health>().Value.ShouldBe(70);
        entity2.Get<Armor>().Value.ShouldBe(6);
        entity3.Get<Health>().Value.ShouldBe(30);
    }

    [Fact]
    public void Iterate_WorksWithOneComponent()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity().With(new Health(10)).With(new Healing(5)).Build();
        var entity2 = world.DefineEntity().With(new Health(20)).Build();

        world.System<Health>().Iterate((Span<Id> ids, Span<Health> healths) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healths[i].Value += 10;
            }
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(20);
        entity2.Get<Health>().Value.ShouldBe(30);
    }

    [Fact]
    public void Iterate_WorksWithTwoComponents()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity().With(new Health(10)).With(new Healing(5)).Build();
        var entity2 = world.DefineEntity().With(new Health(20)).With(new Healing(10)).Build();
        var entity3 = world.DefineEntity().With(new Health(15)).Build();

        world.System<Health, Healing>().Iterate((Span<Id> ids, Span<Health> healths, Span<Healing> healings) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healths[i].Value += healings[i].Amount;
            }
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(15);
        entity2.Get<Health>().Value.ShouldBe(30);
        entity3.Get<Health>().Value.ShouldBe(15);
    }

    [Fact]
    public void Iterate_WorksWithThreeComponents()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity().With(new Health(10)).With(new Healing(5)).With(new Armor(1)).Build();
        var entity2 = world.DefineEntity().With(new Health(20)).With(new Healing(10)).With(new Armor(2)).Build();
        var entity3 = world.DefineEntity().With(new Health(15)).Build();

        world.System<Health, Healing, Armor>().Iterate((Span<Id> ids, Span<Health> healths, Span<Healing> healings, Span<Armor> armors) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healths[i].Value += healings[i].Amount;
                armors[i].Value *= 2;
            }
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(15);
        entity1.Get<Armor>().Value.ShouldBe(2);
        entity2.Get<Health>().Value.ShouldBe(30);
        entity2.Get<Armor>().Value.ShouldBe(4);
        entity3.Get<Health>().Value.ShouldBe(15);
    }
}
