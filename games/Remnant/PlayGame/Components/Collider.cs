using System.Diagnostics;
using System.Numerics;

namespace Remant.PlayGame;

public enum ColliderShape
{
    Sphere,
    Aabb,
}

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct Collider
{
    public readonly ColliderShape Shape;
    public readonly Vector3 HalfExtents;
    public readonly Vector3 Offset;
    public readonly float FootprintRadius;

    private Collider(ColliderShape shape, Vector3 halfExtents, Vector3 offset, float footprintRadius)
    {
        Shape = shape;
        HalfExtents = halfExtents;
        Offset = offset;
        FootprintRadius = footprintRadius;
    }

    public static Collider Sphere(float radius, Vector3 offset = default)
        => new(ColliderShape.Sphere, new Vector3(radius, 0, 0), offset, radius);

    public static Collider Box(Vector3 halfExtents, Vector3 offset = default)
        => new(ColliderShape.Aabb, halfExtents, offset, MathF.Sqrt(halfExtents.X * halfExtents.X + halfExtents.Z * halfExtents.Z));


    public override string ToString()
    {
        return $"Collider({Shape}, {HalfExtents}, {Offset}, {FootprintRadius})";
    }
}
