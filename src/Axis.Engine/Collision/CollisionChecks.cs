using System.Numerics;

namespace Axis.Engine.Collision;

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
        Vector2 delta = aabbB.Center - aabbA.Center;
        Vector2 totalHalfExtents = aabbA.HalfExtents + aabbB.HalfExtents;

        return MathF.Abs(delta.X) <= totalHalfExtents.X &&
               MathF.Abs(delta.Y) <= totalHalfExtents.Y;
    }
}
