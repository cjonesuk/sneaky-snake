using System.Diagnostics;
using System.Numerics;
using Engine.Components;
using Engine.Input;
using Engine.WorldManagement;
using Engine.WorldManagement.Actors;
using Engine.WorldManagement.Entities;
using Raylib_cs;

namespace SneakySnake;


internal sealed class SnakeActor : IActor<SnakeActor.Defaults>, IInputReceiver
{
    private IWorld? _world;
    private EntityId _headId;
    private static readonly float _spacing = 25f;
    private static readonly Vector2 _segmentSize = new Vector2(20f, 20f);


    public record struct Defaults(Transform2d Transform, SnakeControl Control);

    public SnakeActor()
    {
        _headId = EntityId.Invalid;
        _world = null;
    }

    public void Initialize(IWorld world, Defaults properties)
    {
        Debug.Assert(_world == null, "SnakeActor has already been initialized");

        _world = world;

        _headId = world.Entities.AddEntity(
            properties.Transform,
            properties.Control,
            new SnakeSegments(_spacing),
            new BasicShape(ShapeType.Rectangle, _segmentSize, Color.Green)
        );
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        Debug.Assert(_world != null, "SnakeActor has not been initialized");

        var entity = _world.Entities.QueryById(_headId);

        if (inputEvent.Action is SnakeActions.GenericAction)
        {
            AddBodySegment(entity);
        }

        entity.GetRef<SnakeControl>().PendingActions.Add(inputEvent.Action);
    }

    public void Tick(float deltaTime)
    {

    }

    private void AddBodySegment(EntityQueryResult entity)
    {
        Debug.Assert(_world != null, "SnakeActor has not been initialized");

        ref var segments = ref entity.GetRef<SnakeSegments>();

        if (segments.Entities.Count == 0)
        {
            var headTransform = entity.GetCopy<Transform2d>();
            SpawnSegment(segments.Entities, headTransform);

            return;
        }

        var tailEntity = _world.Entities.QueryById(segments.Entities[^1]);
        var tailTransform = tailEntity.GetCopy<Transform2d>();

        SpawnSegment(segments.Entities, tailTransform);
    }

    private void SpawnSegment(List<EntityId> segments, Transform2d transform)
    {
        Debug.Assert(_world != null, "SnakeActor has not been initialized");

        EntityId segmentId = _world.Entities.AddEntity(
            transform,
            new BasicShape(ShapeType.Rectangle, _segmentSize, Color.DarkGreen)
        );

        segments.Add(segmentId);
    }
}
