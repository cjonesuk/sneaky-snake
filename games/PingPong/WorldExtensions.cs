using System.Numerics;
using Axis.ECS;
using Axis.Engine.Components;
using Engine.Components;
using Raylib_cs;
namespace PingPong;

internal static class WorldExtensions
{
    public static Entity SpawnCamera2d(this IWorld world, Vector2 position, float zoom)
    {
        return world.CreateEntity(new Transform2d(position), new Camera2d(zoom));
    }

    public static Entity SpawnBall(this IWorld world, Vector2 position, Color color)
    {
        return world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Circle, new Vector2(20f, 20f),
            color));
    }

    public static Entity SpawnPaddle(this IWorld world, Vector2 position, Color color)
    {
        return world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Rectangle, new Vector2(20f, 100f), color));
    }
}
