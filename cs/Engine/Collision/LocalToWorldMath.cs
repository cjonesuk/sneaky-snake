using System.Numerics;
using Engine.Components;
using Engine.WorldManagement.Entities;

namespace Engine.Collision;

internal static class LocalToWorldMath
{
    public static WorldAabb WorldAabb(
        in EntityId entityId,
        in Transform2d transform,
        in CollisionBody body)
    {
        Vector2 center = transform.Position + body.Offset;
        Vector2 halfExtents = body.HalfExtents;

        return new WorldAabb(entityId, center, halfExtents);
    }

    public static WorldObb WorldObb(
        in EntityId entityId,
        in Transform2d transform,
        in CollisionBody body)
    {
        float radians = MathF.PI / 180f * transform.Rotation;
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);

        Vector2 offset = new(
            body.Offset.X * c - body.Offset.Y * s,
            body.Offset.X * s + body.Offset.Y * c
        );

        Vector2 center = transform.Position + offset;

        Vector2 xAxis = new(c, s);
        Vector2 yAxis = new(-s, c);

        return new WorldObb(entityId, center, xAxis, yAxis, body.HalfExtents);
    }

    public static WorldCircle WorldCircle(
        in EntityId entityId,
        in Transform2d transform,
        in CollisionBody body)
    {
        float radians = MathF.PI / 180f * transform.Rotation;

        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);

        Vector2 offset = new(
            body.Offset.X * c - body.Offset.Y * s,
            body.Offset.X * s + body.Offset.Y * c
        );

        Vector2 center = transform.Position + offset;
        float radius = body.HalfExtents.X;

        return new WorldCircle(entityId, center, radius);
    }
}
