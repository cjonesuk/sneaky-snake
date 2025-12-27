using System.Numerics;
namespace Engine.Collision;

internal static class CollisionChecks
{
    public static bool CircleVsCircle(in WorldCircle a, in WorldCircle b)
    {
        Console.WriteLine($"Checking CircleVsCircle between Circle A at {a.Center} with radius {a.Radius} and Circle B at {b.Center} with radius {b.Radius}");

        Vector2 delta = b.Center - a.Center;

        float distanceSq = Vector2.Dot(delta, delta);
        float radiusSum = a.Radius + b.Radius;

        return distanceSq <= radiusSum * radiusSum;
    }
}
