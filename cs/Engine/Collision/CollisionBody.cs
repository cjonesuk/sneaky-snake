using System.Numerics;
namespace Engine.Collision;

public record struct CollisionBody(CollisionShape Shape, Vector2 HalfExtents, Vector2 Offset);
