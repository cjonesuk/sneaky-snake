using System.Numerics;
using Axis.ECS;

namespace Axis.Engine.Collision;

internal readonly struct WorldCircle
{
    public readonly Id EntityId;
    public readonly Vector2 Center;
    public readonly float Radius;

    public WorldCircle(Id entityId, Vector2 center, float radius)
    {
        EntityId = entityId;
        Center = center;
        Radius = radius;
    }
}
