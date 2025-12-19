using System.Numerics;
using Engine;
using Raylib_cs;

namespace SneakySnake;

public struct BasicShapeRenderCommand
{
    public float X;
    public float Y;
    public float SizeX;
    public float SizeY;
    public Color Color;
    public ShapeType Type;

    public BasicShapeRenderCommand(float x, float y, float sizeX, float sizeY, Color color, ShapeType type)
    {
        X = x;
        Y = y;
        SizeX = sizeX;
        SizeY = sizeY;
        Color = color;
        Type = type;
    }
}
