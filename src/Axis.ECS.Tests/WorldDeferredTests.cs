using Shouldly;

namespace Axis.ECS.Tests;

public class WorldDeferredTests
{
    [Fact]
    public void AddEntity_DefersUntilScopeExit()
    {
        var world = World.Create();

        Entity entity1, entity2, entity3;

        using (world.BeginDeferringCommands())
        {
            entity1 = world.SpawnEntity();
            entity2 = world.DefineEntity().With(new Position(1.0f, 2.0f)).Build();
            entity3 = world.DefineEntity().With(new Position(3.0f, 4.0f)).With(new Velocity(0.5f, 0.25f)).Build();

            entity1.IsAlive().ShouldBeFalse();
            entity2.IsAlive().ShouldBeFalse();
            entity3.IsAlive().ShouldBeFalse();
        }

        entity1.IsAlive().ShouldBeTrue();
        entity2.IsAlive().ShouldBeTrue();
        entity3.IsAlive().ShouldBeTrue();

        entity2.GetRef<Position>().ShouldBe(new Position(1.0f, 2.0f));
        entity3.GetRef<Position>().ShouldBe(new Position(3.0f, 4.0f));
        entity3.GetRef<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void RemoveEntity_DefersUntilScopeExit()
    {
        var world = World.Create();

        var entity1 = world.SpawnEntity();
        var entity2 = world.DefineEntity().With(new Position(1.0f, 2.0f)).Build();

        using (world.BeginDeferringCommands())
        {
            entity1.Delete();

            entity1.IsAlive().ShouldBeTrue();
            entity2.IsAlive().ShouldBeTrue();
        }

        entity1.IsAlive().ShouldBeFalse();
        entity2.IsAlive().ShouldBeTrue();
    }

    [Fact]
    public void SetComponent_DefersUntilScopeExit()
    {
        var world = World.Create();

        var entity = world.DefineEntity()
            .With(new Position(1.0f, 2.0f))
            .With(new Velocity(0.5f, 0.25f))
            .Build();

        using (world.BeginDeferringCommands())
        {
            entity.Set(new Position(10.0f, 20.0f));
            entity.Set(new Velocity(5.0f, 2.5f));

            entity.GetRef<Position>().ShouldBe(new Position(1.0f, 2.0f));
            entity.GetRef<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
        }

        entity.GetRef<Position>().ShouldBe(new Position(10.0f, 20.0f));
        entity.GetRef<Velocity>().ShouldBe(new Velocity(5.0f, 2.5f));
    }

    [Fact]
    public void AddEntity_WithManyComponents_PreservesAllAfterFlush()
    {
        var world = World.Create();

        Entity entity;

        using (world.BeginDeferringCommands())
        {
            entity = world.DefineEntity()
                .With(new Position(1.0f, 2.0f))
                .With(new Velocity(0.5f, 0.25f))
                .With(new Health(10))
                .With(new Healing(3))
                .With(new Armor(50))
                .With(new Mana(100))
                .With(new Stamina(75))
                .With(new PlayerTag())
                .Build();

            entity.IsAlive().ShouldBeFalse();
        }

        entity.IsAlive().ShouldBeTrue();
        entity.GetRef<Position>().ShouldBe(new Position(1.0f, 2.0f));
        entity.GetRef<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
        entity.GetRef<Health>().ShouldBe(new Health(10));
        entity.GetRef<Healing>().ShouldBe(new Healing(3));
        entity.GetRef<Armor>().ShouldBe(new Armor(50));
        entity.GetRef<Mana>().ShouldBe(new Mana(100));
        entity.GetRef<Stamina>().ShouldBe(new Stamina(75));
        entity.Has<PlayerTag>().ShouldBeTrue();
    }
}
