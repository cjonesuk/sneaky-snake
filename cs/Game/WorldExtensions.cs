using System.Numerics;
using Engine.Collision;
using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
using Raylib_cs;

namespace SneakySnake;

internal static class WorldExtensions
{
    public static void SpawnFood(this IWorld world, Vector2 position, Color color)
    {
        world.Entities.AddEntity(
            new Transform2d(position),
            new CollisionBody(CollisionShape.Circle, new Vector2(10f, 10f), Vector2.Zero),
            new BasicShape(ShapeType.Circle, new Vector2(20f, 20f), color),
            new FoodTag());
    }

    public static EntityId SpawnText(this IWorld world, Vector2 position, string text, float fontSize, Color color, TextAlignment alignment)
    {
        return world.Entities.AddEntity(new Transform2d(position), new Text2d(text, fontSize, color, alignment));
    }

    public static SnakeActor SpawnSnake(this IWorld world, Vector2 position)
    {
        var defaults = new SnakeActor.Defaults(
            new Transform2d(position, 0.0f),
            new SnakeControl(
                maxSpeed: 300f,
                acceleration: 200f,
                deceleration: 300f,
                maxTurnRate: 180f)
        );

        return world.Actors.SpawnActor<SnakeActor, SnakeActor.Defaults>(defaults);
    }

    public static EntityId SpawnCamera2d(
        this IWorld world,
        Vector2 position = default,
        float rotation = 0.0f,
        float zoom = 1.0f,
        bool debug = false)
    {
        var transform = new Transform2d(position, rotation);
        var camera2d = new Camera2d(zoom);

        if (debug)
        {
            var debugComponent = new DebugVisual(Color.Magenta);
            return world.Entities.AddEntity(transform, camera2d, debugComponent);
        }

        return world.Entities.AddEntity(transform, camera2d);
    }
}
