using Shouldly;

namespace AnECS.Tests;


file record struct Position(float X, float Y);
file record struct Velocity(float DX, float DY);
file record struct PlayerTag();

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
    public void QueryEntities_wip()
    {
        var world = World.Create();
        var entity1 = world.CreateEntity();
        entity1.Set(new Position(1.0f, 2.0f));

        var entity2 = world.CreateEntity();
        entity2.Set(new Position(3.0f, 4.0f));
        entity2.Set(new Velocity(0.5f, 0.25f));

        entity1.Has<Position>().ShouldBeTrue();
        entity1.Has<Velocity>().ShouldBeFalse();

        entity2.Has<Position>().ShouldBeTrue();
        entity2.Has<Velocity>().ShouldBeTrue();

        QueryRecorder.QueryEach<Position>(world).ShouldBeEquivalentTo(new List<(Id, Position)>
        {
            (entity1.Id, new Position(1.0f, 2.0f)),
            (entity2.Id, new Position(3.0f, 4.0f))
        });

        QueryRecorder.QueryEach<Velocity>(world).ShouldBeEquivalentTo(new List<(Id, Velocity)>
        {
            (entity2.Id, new Velocity(0.5f, 0.25f))
        });

        QueryRecorder.QueryEach<Position, Velocity>(world).ShouldBeEquivalentTo(new List<(Id, Position, Velocity)>
        {
            (entity2.Id, new Position(3.0f, 4.0f), new Velocity(0.5f, 0.25f))
        });

        entity1.Add<PlayerTag>();

        QueryRecorder.QueryEach<PlayerTag>(world).ShouldBeEquivalentTo(new List<(Id, PlayerTag)>
        {
            (entity1.Id, new PlayerTag())
        });
    }
}
