using System.Numerics;
using Engine.WorldManagement.Entities;

namespace Engine.Collision;

internal readonly struct WorldAabb
{
    public readonly EntityId EntityId;
    public readonly Vector2 Center;
    public readonly Vector2 HalfExtents;

    public WorldAabb(EntityId entityId, Vector2 center, Vector2 halfExtents)
    {
        EntityId = entityId;
        Center = center;
        HalfExtents = halfExtents;
    }
}
