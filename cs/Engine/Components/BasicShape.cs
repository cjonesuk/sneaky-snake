using System.Numerics;
using Raylib_cs;

namespace Engine.Components;

public enum ShapeType
{
    Rectangle,
    Circle,
}

public struct BasicShape
{
    public ShapeType Type;
    public Vector2 Size;
    public Color Color;

    public BasicShape(ShapeType type, Vector2 size, Color color)
    {
        Type = type;
        Size = size;
        Color = color;
    }
}
