using System.Numerics;

namespace Engine.Collision;

internal readonly struct WorldObb
{
    public readonly Vector2 Center;
    public readonly Vector2 XAxis; // normalized local X
    public readonly Vector2 YAxis; // normalized local Y
    public readonly Vector2 HalfExtents;

    public WorldObb(Vector2 center, Vector2 xAxis, Vector2 yAxis, Vector2 halfExtents)
    {
        Center = center;
        XAxis = xAxis;
        YAxis = yAxis;
        HalfExtents = halfExtents;
    }
}
