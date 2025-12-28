using System.Numerics;
using Engine.WorldManagement.Entities;

namespace Engine.Collision;

internal readonly struct WorldCircle
{
    public readonly EntityId EntityId;
    public readonly Vector2 Center;
    public readonly float Radius;

    public WorldCircle(EntityId entityId, Vector2 center, float radius)
    {
        EntityId = entityId;
        Center = center;
        Radius = radius;
    }
}
