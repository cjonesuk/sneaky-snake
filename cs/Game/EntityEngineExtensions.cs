using Engine;
using Raylib_cs;

namespace SneakySnake;

public static class EntityEngineExtensions
{
    public static void SpawnFood(this IGameEngine engine, int x, int y, Color color)
    {
        engine.Entities.AddEntity(new GridTransform(x, y), new BasicShape(ShapeType.Circle, color), new FoodTag());
    }

    public static void SpawnText(this IGameEngine engine, float x, float y, string text, float fontSize, Color color, TextAlignment alignment)
    {
        engine.Entities.AddEntity(new Transform2d(x, y), new Text2d(text, fontSize, color, alignment));
    }
}