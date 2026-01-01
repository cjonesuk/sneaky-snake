using System.Numerics;
using Engine.WorldManagement.Entities;

namespace Engine.Collision;

internal readonly struct WorldObb
{
    public readonly EntityId EntityId;
    public readonly Vector2 Center;
    public readonly Vector2 XAxis; // normalized local X
    public readonly Vector2 YAxis; // normalized local Y
    public readonly Vector2 HalfExtents;

    public WorldObb(
        EntityId entityId,
        Vector2 center,
        Vector2 xAxis,
        Vector2 yAxis,
        Vector2 halfExtents)
    {
        EntityId = entityId;
        Center = center;
        XAxis = xAxis;
        YAxis = yAxis;
        HalfExtents = halfExtents;
    }
}
