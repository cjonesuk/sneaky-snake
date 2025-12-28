using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
namespace Engine.Collision;

internal sealed class CollisionSystem : IWorldSystem
{
    private readonly WorldSpaceLists _worldSpace;
    private readonly HashSet<CollisionPair> _collisions;

    public CollisionSystem()
    {
        _worldSpace = WorldSpaceLists.Create();
        _collisions = new HashSet<CollisionPair>();
    }

    public void Update(IWorld world, float deltaTime)
    {
        ComputeWorldspace(world);

        ComputeCollisions(world);


    }

    private void ComputeWorldspace(IWorld world)
    {
        _worldSpace.Clear();

        var result = world.Entities.QueryAllV2<Transform2d, CollisionBody>();

        for (int pageIndex = 0; pageIndex < result.PageCount; pageIndex++)
        {
            var (entityIds, transforms, bodies) = result.GetPage(pageIndex);

            for (int entityIndex = 0; entityIndex < entityIds.Length; entityIndex++)
            {
                ref var entityId = ref entityIds[entityIndex];
                ref var body = ref bodies[entityIndex];
                ref var transform = ref transforms[entityIndex];

                switch (body.Shape)
                {
                    case CollisionShape.Aabb:
                        var worldAabb = LocalToWorldMath.WorldAabb(entityId, transform, body);
                        _worldSpace.Aabbs.Add(worldAabb);
                        break;
                    case CollisionShape.Obb:
                        var worldObb = LocalToWorldMath.WorldObb(entityId, transform, body);
                        _worldSpace.Obbs.Add(worldObb);
                        break;
                    case CollisionShape.Circle:
                        var worldCircle = LocalToWorldMath.WorldCircle(entityId, transform, body);
                        _worldSpace.Circles.Add(worldCircle);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown collision shape: {body.Shape}");
                }
            }
        }
    }


    private void ComputeCollisions(IWorld world)
    {
        _collisions.Clear();

        var collisionEvents = world.Events.GetEventList<CollisionEvent>();

        var aabbs = _worldSpace.GetAabbsSpan();
        var obbs = _worldSpace.GetObbsSpan();
        var circles = _worldSpace.GetCirclesSpan();

        // for (int indexA = 0; indexA < aabbs.Length; indexA++)
        // {
        //     ref var aabbA = ref aabbs[indexA];

        //     for (int indexB = indexA + 1; indexB < aabbs.Length; indexB++)
        //     {
        //         ref var aabbB = ref aabbs[indexB];

        //         if (CollisionChecks.AabbVsAabb(in aabbA, in aabbB))
        //         {
        //             CollisionFound(collisionEvents, aabbA.EntityId, aabbB.EntityId);
        //         }
        //     }
        // }

        // Circle vs Circle
        for (int indexA = 0; indexA < circles.Length; indexA++)
        {
            ref var circleA = ref circles[indexA];

            for (int indexB = indexA + 1; indexB < circles.Length; indexB++)
            {
                ref var circleB = ref circles[indexB];


                if (CollisionChecks.CircleVsCircle(in circleA, in circleB))
                {
                    CollisionFound(collisionEvents, circleA.EntityId, circleB.EntityId);
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
