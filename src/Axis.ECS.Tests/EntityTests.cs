using Shouldly;

namespace Axis.ECS.Tests;


public class EntityTests
{
    [Fact]
    public void Set_AddsAndSetsComponent()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));

        entity.Has<Position>().ShouldBeTrue();
        entity.Has<Velocity>().ShouldBeTrue();
        entity.Has<PlayerTag>().ShouldBeFalse();

        entity.Get<Position>().ShouldBe(new Position(1.0f, 2.0f));
        entity.Get<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void Create_Empty_HasNoComponents()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        var debug = entity.DebugExport();
        debug.EntityType.ShouldBe(EntityType.Empty);
        debug.Components.ShouldBeEmpty();
    }

    [Fact]
    public void Create_OneValue_HasOneComponent()
    {
        var world = World.Create();
        var entity = world.CreateEntity(new Position(1.0f, 2.0f));
        var debug = entity.DebugExport();
        debug.EntityType.ShouldBe(EntityType.From<Position>());
        debug.ComponentCount.ShouldBe(1);
        debug.GetComponent<Position>().ShouldBe(new Position(1.0f, 2.0f));
    }

    [Fact]
    public void Create_TwoValues_HasTwoComponents()
    {
        var world = World.Create();
        var entity = world.CreateEntity(new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f));
        var debug = entity.DebugExport();
        debug.EntityType.ShouldBe(EntityType.From<Position, Velocity>());
        debug.ComponentCount.ShouldBe(2);
        debug.GetComponent<Position>().ShouldBe(new Position(1.0f, 2.0f));
        debug.GetComponent<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void Remove_RemovesComponent()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));
        entity.Has<Position>().ShouldBeTrue();
        entity.Has<Velocity>().ShouldBeTrue();

        entity.Remove<Position>();
        entity.Has<Position>().ShouldBeFalse();
        entity.Has<Velocity>().ShouldBeTrue();

        entity.Add<PlayerTag>();
        entity.Has<PlayerTag>().ShouldBeTrue();

        entity.Remove<PlayerTag>();
        entity.Has<PlayerTag>().ShouldBeFalse();
        entity.Has<Velocity>().ShouldBeTrue();
        entity.Has<Position>().ShouldBeFalse();
    }

    [Fact]
    public void Delete_RemovesEntityFromWorld()
    {
        var world = World.Create();
        var entity = world.CreateEntity(new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f));
        entity.IsAlive().ShouldBeTrue();

        entity.Delete();
        entity.IsAlive().ShouldBeFalse();
    }

    record struct Context();

    [Fact]
    public void GetRef_ReturnsComponentReference()
    {
        var context = new Context();

        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));
        entity.Add<PlayerTag>();

        ref var positionRef = ref entity.GetRef<Position>();
        positionRef.X.ShouldBe(1.0f);
        positionRef.Y.ShouldBe(2.0f);

        positionRef.X = 10.0f;
        positionRef.Y = 20.0f;

        var positions = QueryRecorder.QueryEach<Context, Position>(world, ref context);

        positions.Count.ShouldBe(1);
        positions[0].ShouldBe((entity.Id, new Position(10.0f, 20.0f)));
    }

    [Fact]
    public void Get_ReturnsACopyOfTheComponent()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));

        var position = entity.Get<Position>();
        position.X.ShouldBe(1.0f);
        position.Y.ShouldBe(2.0f);

        // Modify the copy
        position.X = 10.0f;
        position.Y = 20.0f;

        // Get the component again to verify it hasn't changed in the world
        var positionAgain = entity.Get<Position>();
        positionAgain.X.ShouldBe(1.0f);
        positionAgain.Y.ShouldBe(2.0f);
    }

    [Fact]
    public void GetEntityType_ReturnsCorrectEntityType()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));

        var entityType = world.GetEntityType(entity.Id);

        entityType.ShouldBe(EntityType.Create([
            ComponentTypeInformation<Position>.Id,
            ComponentTypeInformation<Velocity>.Id
        ]));
    }
}