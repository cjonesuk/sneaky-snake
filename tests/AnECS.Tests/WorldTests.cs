using Shouldly;

namespace AnECS.Tests;


record struct Position(float X, float Y);
record struct Velocity(float DX, float DY);
record struct PlayerTag();

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
        entity.Set(new Position(1.0f, 2.0f));
        entity.Set(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void QueryEntities_wip()
    {
        var world = World.Create();
        var entity1 = world.Entity();
        entity1.Set(new Position(1.0f, 2.0f));

        var entity2 = world.Entity();
        entity2.Set(new Position(3.0f, 4.0f));
        entity2.Set(new Velocity(0.5f, 0.25f));

        List<(Id, Position)> actualPositions = new();

        world.Query((ref Id id, ref Position position) =>
        {
            actualPositions.Add((id, position));
        });

        actualPositions.Count.ShouldBe(2);
        actualPositions.ShouldBeEquivalentTo(new List<(Id, Position)>
        {
            (entity1.Id, new Position(1.0f, 2.0f)),
            (entity2.Id, new Position(3.0f, 4.0f))
        });

        List<(Id, Velocity)> actualVelocities = new();

        world.Query((ref Id id, ref Velocity velocity) =>
        {
            actualVelocities.Add((id, velocity));
        });

        actualVelocities.Count.ShouldBe(1);
        actualVelocities.ShouldBeEquivalentTo(new List<(Id, Velocity)>
        {
            (entity2.Id, new Velocity(0.5f, 0.25f))
        });

        List<(Id, Position, Velocity)> actualPositionVelocities = new();

        world.Query((ref Id id, ref Position position, ref Velocity velocity) =>
        {
            actualPositionVelocities.Add((id, position, velocity));
        });

        actualPositionVelocities.Count.ShouldBe(1);
        actualPositionVelocities.ShouldBeEquivalentTo(new List<(Id, Position, Velocity)>
        {
            (entity2.Id, new Position(3.0f, 4.0f), new Velocity(0.5f, 0.25f))
        });

        entity1.Add<PlayerTag>();
        List<(Id, PlayerTag)> actualPlayerTags = new();
        world.Query((ref Id id, ref PlayerTag playerTag) =>
        {
            actualPlayerTags.Add((id, playerTag));
        });
        actualPlayerTags.Count.ShouldBe(1);
        actualPlayerTags.ShouldBeEquivalentTo(new List<(Id, PlayerTag)>
        {
            (entity1.Id, new PlayerTag())
        });
    }
}