using Shouldly;

namespace AnECS.Tests;

file record struct Health(int Value);
file record struct Healing(int Amount);
file record struct PlayerTag();

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

        world.System<Health>().ForEach((ref Id id, ref Health health) =>
        {
            totalHealth += health.Value;
        });

        world.System<Healing>().ForEach((ref Id id, ref Healing healing) =>
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

        world.System<Health, Healing>().ForEach((ref Id id, ref Health health, ref Healing healing) =>
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

        world.System<Health>().ForAll((Span<Id> ids, Span<Health> healths) =>
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

        world.System<Health, Healing>().ForAll((Span<Id> ids, Span<Health> healths, Span<Healing> healings) =>
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
}