using Shouldly;

namespace AnECS.Tests;

file record struct Position(float X, float Y);
file record struct Velocity(float DX, float DY);
file record struct PlayerTag();


public class EntityTests
{
    [Fact]
    public void Add_ShouldAddComponentToEntity()
    {
        var world = World.Create();
        var entity = world.CreateEntity();
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));

        entity.Has<Position>().ShouldBeTrue();
        entity.Has<Velocity>().ShouldBeTrue();
        entity.Has<PlayerTag>().ShouldBeFalse();
    }

    [Fact]
    public void Remove_ShouldRemoveComponentFromEntity()
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
    public void GetRef_ShouldReturnComponentReference()
    {
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

        var positions = new List<(Id, Position)>();

        world.Query((ref Id id, ref Position position) =>
        {
            positions.Add((id, position));
        });

        positions.Count.ShouldBe(1);
        positions[0].ShouldBe((entity.Id, new Position(10.0f, 20.0f)));
    }

    [Fact]
    public void Get_ShouldReturnACopyOfTheComponent()
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
}