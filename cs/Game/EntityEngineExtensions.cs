using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Input;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

public static class EntityEngineExtensions
{
    public static void SpawnFood(this IWorld world, Vector2 position, Color color)
    {
        world.Entities.AddEntity(new Transform2d(position), new BasicShape(ShapeType.Circle, color), new FoodTag());
    }

    public static EntityId SpawnText(this IWorld world, Vector2 position, string text, float fontSize, Color color, TextAlignment alignment)
    {
        return world.Entities.AddEntity(new Transform2d(position), new Text2d(text, fontSize, color, alignment));
    }

    public static EntityId SpawnSnake(this IWorld world, Vector2 position)
    {
        return world.Entities.AddEntity(
            new Transform2d(position),
            new SnakeControl(50f, 90f),
            new BasicShape(ShapeType.Rectangle, Color.Green)
        );
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