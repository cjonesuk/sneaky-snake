using System.Numerics;

namespace Engine.Collision;

internal readonly struct WorldCircle
{
    public readonly Vector2 Center;
    public readonly float Radius;

    public WorldCircle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }
}
