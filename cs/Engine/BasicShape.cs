using Raylib_cs;

namespace Engine;

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