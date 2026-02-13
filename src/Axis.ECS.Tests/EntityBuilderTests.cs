using Axis.ECS.Tests;
using Shouldly;

namespace Axis.ECS.Commands;


public class EntityBuilderTests
{
    [Fact]
    public void DefineEntity_CreatesAnEmptyEntity()
    {
        var world = World.Create();
        var builder = world.DefineEntity();
        var entity = builder.Build();

        entity.DebugExport().EntityType.ShouldBe(EntityType.Empty);
    }

    [Fact]
    public void DefineEntity_WithComponents_CreatesEntityWithComponents()
    {
        var world = World.Create();
        var builder = world.DefineEntity()
            .With(new Position(1.0f, 2.0f))
            .With(new Velocity(0.5f, 0.25f));
        var entity = builder.Build();

        var debug = entity.DebugExport();
        debug.EntityType.ShouldBe(EntityType.Create([
            world.Components.GetId<Position>(),
            world.Components.GetId<Velocity>()
        ]));
        debug.ComponentCount.ShouldBe(2);
        debug.GetComponent<Position>().ShouldBe(new Position(1.0f, 2.0f));
        debug.GetComponent<Velocity>().ShouldBe(new Velocity(0.5f, 0.25f));
    }

    [Fact]
    public void WithDuplicateComponents_ThrowsError()
    {
        var world = World.Create();

        Should.Throw<InvalidOperationException>(() =>
        {
            world
              .DefineEntity()
              .With(new Position(1.0f, 2.0f))
              .With(new Position(3.0f, 4.0f))
              .Build();
        });
    }

    [Fact]
    public void Build_ThrowsIfCalledMultipleTimes()
    {
        var world = World.Create();

        Should.Throw<InvalidOperationException>(() =>
        {
            var builder = world.DefineEntity();
            builder.Build();
            builder.Build();
        });
    }

}