using System.Numerics;
namespace Engine.Collision;

internal static class CollisionChecks
{
    public static bool CircleVsCircle(in WorldCircle a, in WorldCircle b)
    {
        Vector2 delta = b.Center - a.Center;

        float distanceSq = Vector2.Dot(delta, delta);
        float radiusSum = a.Radius + b.Radius;

        return distanceSq <= radiusSum * radiusSum;
    }

    internal static bool AabbVsAabb(in WorldAabb aabbA, in WorldAabb aabbB)
    {
        throw new NotImplementedException();
    }
}
