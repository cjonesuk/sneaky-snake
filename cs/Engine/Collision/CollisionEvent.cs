using Engine.WorldManagement.Entities;
namespace Engine.Collision;

public record struct CollisionEvent(EntityId EntityA, EntityId EntityB);
