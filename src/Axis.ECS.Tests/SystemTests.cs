using Shouldly;

namespace Axis.ECS.Tests;

public class SystemTests
{
    [Fact]
    public void ForEach_WorksWithOneComponent()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity(new Health(100), new Healing(30));
        var entity2 = world.CreateEntity(new Health(50));

        int totalHealth = 0;
        int totalHealing = 0;

        world.System<Health>().ForEach((ref context, ref iter, ref health) =>
        {
            totalHealth += health.Value;
        });

        world.System<Healing>().ForEach((ref context, ref iter, ref healing) =>
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
        var entity1 = world.CreateEntity(new Health(100), new Healing(30));
        var entity2 = world.CreateEntity(new Health(50), new Healing(20));
        var entity3 = world.CreateEntity(new Health(30));

        world.System<Health, Healing>().ForEach((ref context, ref iter, ref health, ref healing) =>
        {
            health.Value += healing.Amount;
        });

        world.ExecuteSystems(10.0f);

        entity1.Get<Health>().Value.ShouldBe(130);
        entity2.Get<Health>().Value.ShouldBe(70);
        entity3.Get<Health>().Value.ShouldBe(30);
    }

    [Fact]
    public void ForAll_WorksWithOneComponent()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity(new Health(10), new Healing(5));
        var entity2 = world.CreateEntity(new Health(20));

        world.System<Health>().ForAll((ref context, ids, healths) =>
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
    public void ForAll_WorksWithTwoComponents()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity(new Health(10), new Healing(5));
        var entity2 = world.CreateEntity(new Health(20), new Healing(10));
        var entity3 = world.CreateEntity(new Health(15));

        world.System<Health, Healing>().ForAll((ref context, ids, healths, healings) =>
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
    public void ForEach_WorksWithThreeComponents()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity(new Health(100), new Healing(30), new Armor(10));
        var entity2 = world.CreateEntity(new Health(50), new Healing(20), new Armor(5));
        var entity3 = world.CreateEntity(new Health(30));

        world.System<Health, Healing, Armor>().ForEach((ref context, ref iter, ref health, ref healing, ref armor) =>
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
    public void ForAll_WorksWithThreeComponents()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity(new Health(10), new Healing(5), new Armor(1));
        var entity2 = world.CreateEntity(new Health(20), new Healing(10), new Armor(2));
        var entity3 = world.CreateEntity(new Health(15));

        world.System<Health, Healing, Armor>().ForAll((ref context, ids, healths, healings, armors) =>
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