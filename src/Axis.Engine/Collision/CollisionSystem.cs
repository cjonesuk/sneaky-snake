using Axis.ECS;
using Engine.Components;

namespace Axis.Engine.Collision;

public sealed class CollisionSystem : IWorldSystem
{
    private readonly WorldSpaceLists _worldSpace;

    private HashSet<CollisionPair> _currentCollisions = new();
    private HashSet<CollisionPair> _previousCollisions = new();

    public CollisionSystem()
    {
        _worldSpace = WorldSpaceLists.Create();
    }

    public void Execute(ref WorldSystemContext data)
    {
        var world = data.World;

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

        CollisionSystem refthis = this;
 

        world.QueryEach(ref refthis, static (ref CollisionSystem sys, ref Iter iter, ref Transform2d transform, ref CollisionBody body) =>
        {
            ref Id entityId = ref iter.Id;

            switch (body.Shape)
            {
                case CollisionShape.Aabb:
                    var worldAabb = LocalToWorldMath.WorldAabb(entityId, transform, body);
                    sys._worldSpace.Aabbs.Add(worldAabb);
                    break;
                case CollisionShape.Obb:
                    var worldObb = LocalToWorldMath.WorldObb(entityId, transform, body);
                    sys._worldSpace.Obbs.Add(worldObb);
                    break;
                case CollisionShape.Circle:
                    var worldCircle = LocalToWorldMath.WorldCircle(entityId, transform, body);
                    sys._worldSpace.Circles.Add(worldCircle);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown collision shape: {body.Shape}");
            }
        });
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
                    _currentCollisions.Add(pair);
                }
            }
        }

        var aabbs = _worldSpace.GetAabbsSpan();

        for (int indexA = 0; indexA < aabbs.Length; indexA++)
        {
            ref var aabbA = ref aabbs[indexA];

            for (int indexB = indexA + 1; indexB < aabbs.Length; indexB++)
            {
                ref var aabbB = ref aabbs[indexB];

                if (CollisionChecks.AabbVsAabb(in aabbA, in aabbB))
                {
                    var pair = CreateOrderedPair(aabbA.EntityId, aabbB.EntityId);
                    _currentCollisions.Add(pair);
                }
            }
        }
    }

    private void EmitCollisionEvents(IWorld world)
    {
        var startedEvents = world.Events.GetEventStream<CollisionStartedEvent>();
        var endedEvents = world.Events.GetEventStream<CollisionEndedEvent>();
        var ongoingEvents = world.Events.GetEventStream<CollisionEvent>();

        foreach (var pair in _currentCollisions)
        {
            if (!_previousCollisions.Contains(pair))
            {
                startedEvents.AddEvent(new CollisionStartedEvent(pair.EntityA, pair.EntityB));
                world.Events.GetEventStream<CollisionStartedWithEvent>(pair.EntityA).AddEvent(new CollisionStartedWithEvent(pair.EntityB));
                world.Events.GetEventStream<CollisionStartedWithEvent>(pair.EntityB).AddEvent(new CollisionStartedWithEvent(pair.EntityA));
            }

            ongoingEvents.AddEvent(new CollisionEvent(pair.EntityA, pair.EntityB));
            world.Events.GetEventStream<CollisionWithEvent>(pair.EntityA).AddEvent(new CollisionWithEvent(pair.EntityB));
            world.Events.GetEventStream<CollisionWithEvent>(pair.EntityB).AddEvent(new CollisionWithEvent(pair.EntityA));
        }

        foreach (var pair in _previousCollisions)
        {
            if (!_currentCollisions.Contains(pair))
            {
                endedEvents.AddEvent(new CollisionEndedEvent(pair.EntityA, pair.EntityB));
                world.Events.GetEventStream<CollisionEndedWithEvent>(pair.EntityA).AddEvent(new CollisionEndedWithEvent(pair.EntityB));
                world.Events.GetEventStream<CollisionEndedWithEvent>(pair.EntityB).AddEvent(new CollisionEndedWithEvent(pair.EntityA));
            }
        }
    }

    private static CollisionPair CreateOrderedPair(Id a, Id b)
        => a < b ? new CollisionPair(a, b) : new CollisionPair(b, a);

}
