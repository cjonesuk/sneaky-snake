using Raylib_cs;

namespace Engine;

public struct Transform
{
    public int X;
    public int Y;

    public Transform(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public enum ShapeType
{
    Rectangle,
    Circle,
}

public struct BasicShape
{
    public ShapeType Type;
    public Color Color;

    public BasicShape(ShapeType type, Color color)
    {
        Type = type;
        Color = color;
    }
}

public struct FoodTag
{

}