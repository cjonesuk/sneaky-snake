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
    public Vector2 HalfExtents;
    public Color Color;

    public BasicShape(ShapeType type, Vector2 halfExtents, Color color)
    {
        Type = type;
        HalfExtents = halfExtents;
        Color = color;
    }
}
