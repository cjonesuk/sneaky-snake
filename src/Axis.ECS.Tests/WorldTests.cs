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

    record struct Context();

    [Fact]
    public void QueryEntities_wip()
    {
        var context = new Context();

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

        QueryRecorder.QueryEach<Context, Position>(world, ref context).ShouldBeEquivalentTo(new List<(Id, Position)>
        {
            (entity1.Id, new Position(1.0f, 2.0f)),
            (entity2.Id, new Position(3.0f, 4.0f))
        });

        QueryRecorder.QueryEach<Context, Velocity>(world, ref context).ShouldBeEquivalentTo(new List<(Id, Velocity)>
        {
            (entity2.Id, new Velocity(0.5f, 0.25f))
        });

        QueryRecorder.QueryEach<Context, Position, Velocity>(world, ref context).ShouldBeEquivalentTo(new List<(Id, Position, Velocity)>
        {
            (entity2.Id, new Position(3.0f, 4.0f), new Velocity(0.5f, 0.25f))
        });

        entity1.Add<PlayerTag>();

        QueryRecorder.QueryEach<Context, PlayerTag>(world, ref context).ShouldBeEquivalentTo(new List<(Id, PlayerTag)>
        {
            (entity1.Id, new PlayerTag())
        });
    }

    [Fact]
    public void ClearAll_RemovesAllEntitiesAndComponents()
    {
        var context = new Context();
        var world = World.Create();
        var entity1 = world.CreateEntity(new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f));
        var entity2 = world.CreateEntity(new Position(3.0f, 4.0f));

        world.RemoveAllEntities();

        entity1.IsAlive().ShouldBeFalse();
        entity2.IsAlive().ShouldBeFalse();

        QueryRecorder.QueryEach<Context, Position>(world, ref context).ShouldBeEmpty();
        QueryRecorder.QueryEach<Context, Velocity>(world, ref context).ShouldBeEmpty();
    }

    [Fact]
    public void QueryEntities_WithUpToFiveComponents()
    {
        var context = new Context();

        var world = World.Create();

        var entity1 = world.CreateEntity(new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f), new Health(10));
        var entity2 = world.CreateEntity(new Position(3.0f, 4.0f), new Velocity(1.0f, 0.5f), new Health(20), new Healing(5), new Armor(100));

        // 3-component query returns both entities
        QueryRecorder.QueryEach<Context, Position, Velocity, Health>(world, ref context).ShouldBeEquivalentTo(
            new List<(Id, Position, Velocity, Health)>
            {
                (entity1.Id, new Position(1.0f, 2.0f), new Velocity(0.5f, 0.25f), new Health(10)),
                (entity2.Id, new Position(3.0f, 4.0f), new Velocity(1.0f, 0.5f), new Health(20))
            });

        // 4-component query returns only entity2
        QueryRecorder.QueryEach<Context, Position, Velocity, Health, Healing>(world, ref context).ShouldBeEquivalentTo(
            new List<(Id, Position, Velocity, Health, Healing)>
            {
                (entity2.Id, new Position(3.0f, 4.0f), new Velocity(1.0f, 0.5f), new Health(20), new Healing(5))
            });

        // 5-component query returns only entity2
        QueryRecorder.QueryEach<Context, Position, Velocity, Health, Healing, Armor>(world, ref context).ShouldBeEquivalentTo(
            new List<(Id, Position, Velocity, Health, Healing, Armor)>
            {
                (entity2.Id, new Position(3.0f, 4.0f), new Velocity(1.0f, 0.5f), new Health(20), new Healing(5), new Armor(100))
            });
    }

    [Fact]
    public void CreateEntity_WithUpToEightComponents_SetsAllComponents()
    {
        var world = World.Create();

        var entity = world.CreateEntity(
            new Position(1.0f, 2.0f),
            new Velocity(0.5f, 0.25f),
            new Health(10),
            new Healing(3),
            new Armor(50),
            new Mana(100),
            new Stamina(75),
            new PlayerTag());

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
