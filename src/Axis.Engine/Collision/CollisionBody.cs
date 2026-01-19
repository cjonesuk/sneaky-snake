using System.Numerics;

namespace Axis.Engine.Collision;

public record struct CollisionBody(CollisionShape Shape, Vector2 HalfExtents, Vector2 Offset);
