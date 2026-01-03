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

    [Fact]
    public void QueryEntities_wip()
    {
        var world = World.Create();
        var entity1 = world.Entity();
        entity1.AddComponent(new Position(1.0f, 2.0f));

        var entity2 = world.Entity();
        entity2.AddComponent(new Position(3.0f, 4.0f));
        entity2.AddComponent(new Velocity(0.5f, 0.25f));

        List<(Id, Position)> actual = new();

        world.Query((ref Id id, ref Position position) =>
        {
            actual.Add((id, position));
        });

        actual.Count.ShouldBe(2);
        actual.ShouldBeEquivalentTo(new List<(Id, Position)>
        {
            (entity1.Id, new Position(1.0f, 2.0f)),
            (entity2.Id, new Position(3.0f, 4.0f))
        });

    }
}