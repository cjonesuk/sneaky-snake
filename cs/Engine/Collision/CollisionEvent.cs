using Engine.WorldManagement.Entities;
namespace Engine.Collision;

public record struct CollisionEvent(EntityId EntityA, EntityId EntityB);
public record struct CollisionStartedEvent(EntityId EntityA, EntityId EntityB);
public record struct CollisionEndedEvent(EntityId EntityA, EntityId EntityB);
