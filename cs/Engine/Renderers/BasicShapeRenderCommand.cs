using System.Numerics;
using Engine.Components;
using Raylib_cs;

namespace SneakySnake;

public struct BasicShapeRenderCommand
{
    public Vector2 Position;
    public Vector2 Size;
    public float Rotation;
    public Color Color;
    public ShapeType Type;

    public BasicShapeRenderCommand(Vector2 position, Vector2 size, float rotation, Color color, ShapeType type)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
        Color = color;
        Type = type;
    }
}
