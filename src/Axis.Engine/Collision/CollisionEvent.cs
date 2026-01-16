using Axis.ECS;
namespace Axis.Engine.Collision;

public record struct CollisionEvent(Id EntityA, Id EntityB);
public record struct CollisionStartedEvent(Id EntityA, Id EntityB);
public record struct CollisionEndedEvent(Id EntityA, Id EntityB);
