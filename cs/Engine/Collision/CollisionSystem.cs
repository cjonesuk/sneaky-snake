using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
namespace Engine.Collision;

internal sealed class CollisionSystem : IWorldSystem
{
    private readonly WorldSpaceLists _worldSpace;

    private HashSet<CollisionPair> _currentCollisions = new();
    private HashSet<CollisionPair> _previousCollisions = new();

    public CollisionSystem()
    {
        _worldSpace = WorldSpaceLists.Create();
    }

    public void Update(IWorld world, float deltaTime)
    {
        ComputeWorldspace(world);

        ComputeCollisions();

        EmitCollisionEvents(world);

        // Prepare previous for reuse before we swap
        _previousCollisions.Clear();

        // Swap sets, but don't clear current afterward — we’ll reuse the now-empty one next frame
        (_previousCollisions, _currentCollisions) = (_currentCollisions, _previousCollisions);
    }

    private void ComputeWorldspace(IWorld world)
    {
        _worldSpace.Clear();

        var result = world.Entities.QueryAll<Transform2d, CollisionBody>();

        foreach (var (entityIds, transforms, bodies) in result)
        {
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

    private void ComputeCollisions()
    {
        var circles = _worldSpace.GetCirclesSpan();

        for (int indexA = 0; indexA < circles.Length; indexA++)
        {
            ref var circleA = ref circles[indexA];

            for (int indexB = indexA + 1; indexB < circles.Length; indexB++)
            {
                ref var circleB = ref circles[indexB];

                if (CollisionChecks.CircleVsCircle(in circleA, in circleB))
                {
                    var pair = CreateOrderedPair(circleA.EntityId, circleB.EntityId);
                    //var pair = new CollisionPair(circleA.EntityId, circleB.EntityId);
                    _currentCollisions.Add(pair);
                }
            }
        }
    }

    private void EmitCollisionEvents(IWorld world)
    {
        var startedEvents = world.Events.GetEventList<CollisionStartedEvent>();
        var endedEvents = world.Events.GetEventList<CollisionEndedEvent>();
        var ongoingEvents = world.Events.GetEventList<CollisionEvent>();

        foreach (var pair in _currentCollisions)
        {
            if (!_previousCollisions.Contains(pair))
            {
                startedEvents.Add(new CollisionStartedEvent(pair.EntityA, pair.EntityB));
            }

            ongoingEvents.Add(new CollisionEvent(pair.EntityA, pair.EntityB));
        }

        foreach (var pair in _previousCollisions)
        {
            if (!_currentCollisions.Contains(pair))
            {
                endedEvents.Add(new CollisionEndedEvent(pair.EntityA, pair.EntityB));
            }
        }
    }

    private static CollisionPair CreateOrderedPair(EntityId a, EntityId b)
        => a < b ? new CollisionPair(a, b) : new CollisionPair(b, a);
}