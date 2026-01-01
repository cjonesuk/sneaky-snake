using Engine.WorldManagement.Entities;
namespace Engine.Collision;

internal record struct CollisionPair(EntityId EntityA, EntityId EntityB);
