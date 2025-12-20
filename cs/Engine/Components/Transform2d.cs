using System.Numerics;

namespace Engine.Components;

public struct Transform2d
{
    public Vector2 Position;
    public float Rotation;

    public Transform2d(Vector2 position, float rotation = 0.0f)
    {
        Position = position;
        Rotation = rotation;
    }
}
