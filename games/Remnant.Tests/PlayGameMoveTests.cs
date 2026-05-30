using System.Numerics;
using Axis.ECS;
using Axis.Engine.Components;
using Raylib_cs;
using Remnant.PlayGame;
using Shouldly;

namespace Remnant.Tests;

/// <summary>
/// Headless repro for the select→move→re-select→move loop. Exercises <see cref="MovementSystem"/>
/// against a real world while standing in for <see cref="SelectionSystem"/> and
/// <see cref="MoveCommandSystem"/> with direct entity mutations inside deferred scopes
/// (same observable effect as the input-driven systems, no Raylib/window required).
/// </summary>
public class PlayGameMoveTests
{
    [Fact]
    public void SecondCube_GoesToItsOwnTarget_NotTheFirstCubesPreviousTarget()
    {
        var world = World.Create();
        world.AddSystem(new MovementSystem());

        var cubeA = SpawnCube(world, new Vector3(0f, 0.5f, 0f));
        var cubeB = SpawnCube(world, new Vector3(10f, 0.5f, 0f));

        var targetA = new Vector3(5f, 0.5f, 5f);
        SelectAndCommandMove(world, cubeA, targetA);
        RunMovementToArrival(world, cubeA);
        cubeA.GetRef<Transform3d>().Position.ShouldBe(targetA);

        Deselect(world, cubeA);

        var targetB = new Vector3(-3f, 0.5f, 2f);
        SelectAndCommandMove(world, cubeB, targetB);

        cubeB.GetRef<MoveTarget>().Position.ShouldBe(targetB);

        RunMovementToArrival(world, cubeB);
        cubeB.GetRef<Transform3d>().Position.ShouldBe(targetB);
    }

    private static Entity SpawnCube(World world, Vector3 position)
    {
        var entity = world.SpawnEntity();
        entity.Set(new Transform3d(position));
        entity.Set(new BasicShape3d(Shape3dKind.Cube, Vector3.One, Color.White));
        entity.Add<Selectable>();
        return entity;
    }

    private static void SelectAndCommandMove(World world, Entity entity, Vector3 target)
    {
        using (world.BeginDeferringCommands())
        {
            entity.Add<Selected>();
            entity.Set(new Outline(Color.Yellow));
            entity.Set(new MoveTarget(target));
        }
    }

    private static void Deselect(World world, Entity entity)
    {
        using (world.BeginDeferringCommands())
        {
            entity.Remove<Selected>();
            entity.Remove<Outline>();
        }
    }

    private static void RunMovementToArrival(World world, Entity entity, int maxTicks = 200)
    {
        for (int i = 0; i < maxTicks; i++)
        {
            world.ExecuteSystems(0.1f);
            if (!entity.Has<MoveTarget>()) return;
        }
        throw new InvalidOperationException($"Entity {entity.Id} did not arrive within {maxTicks} ticks.");
    }
}
