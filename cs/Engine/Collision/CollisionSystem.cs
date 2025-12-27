using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
namespace Engine.Collision;

internal sealed class CollisionSystem : IWorldSystem
{
    private readonly HashSet<CollisionPair> _collisions;

    public CollisionSystem()
    {
        _collisions = new HashSet<CollisionPair>();
    }

    public void Update(IWorld world, float deltaTime)
    {
        _collisions.Clear();

        var pages = world.Entities.QueryAll<Transform2d, CollisionBody>();

        var collisionEvents = world.Events.GetEventList<CollisionEvent>();

        // todo: Need to make lists from each page into one cohesive list of entities to check




        foreach (var (entityIds, transforms, bodies) in pages)
        {
            for (int indexA = 0; indexA < entityIds.Count; indexA++)
            {
                EntityId entityA = entityIds[indexA];
                Transform2d transformA = transforms[indexA];
                CollisionBody bodyA = bodies[indexA];

                for (int indexB = indexA + 1; indexB < entityIds.Count; indexB++)
                {
                    EntityId entityB = entityIds[indexB];
                    Transform2d transformB = transforms[indexB];
                    CollisionBody bodyB = bodies[indexB];

                    Console.WriteLine($"Checking collision between Entity {entityA.Id} and Entity {entityB.Id}");

                    if (bodyA.Shape == CollisionShape.Circle)
                    {
                        var worldCircleA = LocalToWorldMath.WorldCircle(transformA, bodyA);

                        if (bodyB.Shape == CollisionShape.Circle)
                        {
                            var worldCircleB = LocalToWorldMath.WorldCircle(transformB, bodyB);

                            bool collision = CollisionChecks.CircleVsCircle(worldCircleA, worldCircleB);

                            if (collision)
                            {
                                CollisionFound(collisionEvents, entityA, entityB);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CollisionFound(List<CollisionEvent> collisionEvents, EntityId entityA, EntityId entityB)
    {
        var pair = new CollisionPair(entityA, entityB);
        bool added = _collisions.Add(pair);

        if (added)
        {
            collisionEvents.Add(new CollisionEvent(entityA, entityB));
        }
    }
}
