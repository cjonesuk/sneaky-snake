using Axis.ECS;
namespace Axis.Engine.Collision;

public record struct CollisionEvent(Id EntityA, Id EntityB);
public record struct CollisionStartedEvent(Id EntityA, Id EntityB);
public record struct CollisionEndedEvent(Id EntityA, Id EntityB);

public record struct CollisionWithEvent(Id EntityId);
public record struct CollisionStartedWithEvent(Id EntityId);
public record struct CollisionEndedWithEvent(Id EntityId);