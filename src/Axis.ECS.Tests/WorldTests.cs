using Shouldly;

namespace Axis.ECS.Tests;

public class WorldTests
{
    [Fact]
    public void CreateWorld_ShouldNotBeNull()
    {
        var world = World.Create();
        world.ShouldNotBeNull();
    }

    [Fact]
    public void CreateEntity_ShouldReturnValidEntity()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.World.ShouldBe(world);
        entity.Id.Value.ShouldNotBe<ulong>(0);
    }

    [Fact]
    public void RemoveAllEntities_RemovesAllEntitiesAndComponents()
    {
        var world = World.Create();
        var entity1 = world.DefineEntity()
            .With(new Position(1.0f, 2.0f))
            .With(new Velocity(0.5f, 0.25f))
            .Build();
        var entity2 = world.DefineEntity()
            .With(new Position(3.0f, 4.0f))
            .Build();

        world.RemoveAllEntities();

        entity1.IsAlive().ShouldBeFalse();
        entity2.IsAlive().ShouldBeFalse();
    }

    [Fact]
    public void DefineEntity_WithEightComponents_SetsAllComponents()
    {
        var world = World.Create();

        var entity = world.DefineEntity()
            .With(new Position(1.0f, 2.0f))
            .With(new Velocity(0.5f, 0.25f))
            .With(new Health(10))
            .With(new Healing(3))
            .With(new Armor(50))
            .With(new Mana(100))
            .With(new Stamina(75))
            .With(new PlayerTag())
            .Build();

        entity.Get<Position>().ShouldBe(new Position(1.0f, 2.0f));
        entity.Get<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
        entity.Get<Health>().ShouldBe(new Health(10));
        entity.Get<Healing>().ShouldBe(new Healing(3));
        entity.Get<Armor>().ShouldBe(new Armor(50));
        entity.Get<Mana>().ShouldBe(new Mana(100));
        entity.Get<Stamina>().ShouldBe(new Stamina(75));
        entity.Has<PlayerTag>().ShouldBeTrue();
    }
}
