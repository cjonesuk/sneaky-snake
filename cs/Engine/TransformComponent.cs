using Raylib_cs;

namespace Engine;

public struct GridTransform
{
    public int X;
    public int Y;

    public GridTransform(int x, int y)
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