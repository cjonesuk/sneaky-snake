using System.Numerics;

namespace Engine.Collision;

internal readonly struct WorldAabb
{
    public readonly Vector2 Center;
    public readonly Vector2 HalfExtents;

    public WorldAabb(Vector2 center, Vector2 halfExtents)
    {
        Center = center;
        HalfExtents = halfExtents;
    }
}
