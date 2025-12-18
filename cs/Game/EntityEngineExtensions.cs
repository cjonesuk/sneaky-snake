using Engine;
using Engine.Components;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

public static class EntityEngineExtensions
{
    public static void SpawnFood(this IWorld world, int x, int y, Color color)
    {
        world.Entities.AddEntity(new GridTransform(x, y), new BasicShape(ShapeType.Circle, color), new FoodTag());
    }

    public static EntityId SpawnMovingFood(this IWorld world, int x, int y, Color color)
    {
        return world.Entities.AddEntity(new GridTransform(x, y), new GridAnimation(1, 0), new BasicShape(ShapeType.Rectangle, color), new FoodTag());
    }

    public static EntityId SpawnText(this IWorld world, float x, float y, string text, float fontSize, Color color, TextAlignment alignment)
    {
        return world.Entities.AddEntity(new Transform2d(x, y), new Text2d(text, fontSize, color, alignment));
    }

    public static EntityId SpawnCamera2d(this IWorld world)
    {
        return world.Entities.AddEntity(new Transform2d(0, 0), new Camera2d());

    }
}