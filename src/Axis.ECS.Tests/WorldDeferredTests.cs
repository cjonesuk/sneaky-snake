using Shouldly;

namespace Axis.ECS.Tests;

file record struct Position(float X, float Y);
file record struct Velocity(float DX, float DY);
file record struct PlayerTag();

public class WorldDeferredTests
{
    [Fact]
    public void AddEntity_AddsEntity()
    {
        var world = World.Create();

        var CreateEntitiesInDeferredContext = () =>
        {
            using var scope = world.BeginDeferringCommands();

            var entity1 = world.CreateEntity();
            var entity2 = world.CreateEntity(new Position(1.0f, 2.0f));
            var entity3 = world.CreateEntity(new Position(3.0f, 4.0f), new Velocity(0.5f, 0.25f));

            entity1.IsAlive().ShouldBeFalse();
            entity2.IsAlive().ShouldBeFalse();
            entity3.IsAlive().ShouldBeFalse();

            QueryRecorder.QueryEach(world).ShouldBeEmpty();

            return (entity1, entity2, entity3);
        };

        var (e1, e2, e3) = CreateEntitiesInDeferredContext();

        var allEntities = QueryRecorder.QueryEach(world);
        allEntities.ShouldBe(new List<Id> { e1.Id, e2.Id, e3.Id }, ignoreOrder: true);

        e2.GetRef<Position>().ShouldBe(new Position(1.0f, 2.0f));
        e3.GetRef<Position>().ShouldBe(new Position(3.0f, 4.0f));
        e3.GetRef<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void RemoveEntity_RemovesEntity()
    {
        var world = World.Create();

        var entity1 = world.CreateEntity();
        var entity2 = world.CreateEntity(new Position(1.0f, 2.0f));

        var RemoveEntitiesInDeferredContext = () =>
        {
            using var scope = world.BeginDeferringCommands();

            entity1.Delete();

            entity1.IsAlive().ShouldBeTrue();
            entity2.IsAlive().ShouldBeTrue();
        };

        RemoveEntitiesInDeferredContext();

        entity1.IsAlive().ShouldBeFalse();
        entity2.IsAlive().ShouldBeTrue();
    }

    [Fact]
    public void SetComponent_SetsComponent()
    {
        var world = World.Create();

        var entity = world.CreateEntity(new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f));

        var SetComponentInDeferredContext = () =>
        {
            using var scope = world.BeginDeferringCommands();

            entity.Set(new Position(10.0f, 20.0f));
            entity.Set(new Velocity(5.0f, 2.5f));

            entity.GetRef<Position>().ShouldBe(new Position(1.0f, 2.0f));
            entity.GetRef<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
        };

        SetComponentInDeferredContext();

        entity.GetRef<Position>().ShouldBe(new Position(10.0f, 20.0f));
        entity.GetRef<Velocity>().ShouldBe(new Velocity(5.0f, 2.5f));
    }
}