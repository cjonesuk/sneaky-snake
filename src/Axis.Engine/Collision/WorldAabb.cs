using System.Numerics;
using Axis.ECS;

namespace Axis.Engine.Collision;

internal readonly struct WorldAabb
{
    public readonly Id EntityId;
    public readonly Vector2 Center;
    public readonly Vector2 HalfExtents;

    public WorldAabb(Id entityId, Vector2 center, Vector2 halfExtents)
    {
        EntityId = entityId;
        Center = center;
        HalfExtents = halfExtents;
    }
}
