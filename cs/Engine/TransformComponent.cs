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

public struct GridAnimation
{
    public int SpeedX;
    public int SpeedY;

    public GridAnimation(int speedX = 0, int speedY = 0)
    {
        SpeedX = speedX;
        SpeedY = speedY;
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