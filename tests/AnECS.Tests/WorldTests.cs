using Shouldly;

namespace AnECS.Tests;


record struct Position(float X, float Y);
record struct Velocity(float DX, float DY);

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
        var entity = world.Entity();
        entity.World.ShouldBe(world);
        entity.Id.Value.ShouldNotBe<ulong>(0);
    }

    [Fact]
    public void AddComponent_ShouldAddComponentToEntity()
    {
        var world = World.Create();
        var entity = world.Entity();
        entity.AddComponent(new Position(1.0f, 2.0f));
        entity.AddComponent(new Velocity(0.5f, 0.25f));
    }
}