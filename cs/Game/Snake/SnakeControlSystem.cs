using System.Numerics;
using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
using Raylib_cs;

namespace SneakySnake;

internal static class SnakeCommands
{
    internal record struct AddSnakeSegmentCommand(EntityId EntityId);
}


internal sealed class SnakeControlSystem : IWorldSystem
{
    private readonly float _friction = 25f;
    private static readonly Vector2 _segmentHalfSize = new Vector2(10f, 10f);
    private readonly List<SnakeCommands.AddSnakeSegmentCommand> _pendingAddSegmentCommands = new();


    public void Update(IWorld world, float deltaTime)
    {
        var result = world.Entities.QueryAll<Transform2d, SnakeControl, InputActionReceiver>();

        foreach (var (entityIds, transforms, controls, inputReceivers) in result)
        {
            for (int index = 0; index < transforms.Length; index++)
            {
                ref var entityId = ref entityIds[index];
                ref var transform = ref transforms[index];
                ref var control = ref controls[index];
                ref var inputReceiver = ref inputReceivers[index];

                ProcessSnakeHead(deltaTime, entityId, ref transform, ref control, inputReceiver);

                ProcessSnakeBody(world, deltaTime, ref control, ref transform);
            }
        }

        SpawnNewSnakeBodySegments(world);
    }


    private void ProcessSnakeHead(float deltaTime, EntityId entityId, ref Transform2d transform, ref SnakeControl control, InputActionReceiver inputReceiver)
    {
        control.Speed = MathF.Max(0, control.Speed - _friction * deltaTime);

        foreach (var action in inputReceiver.PendingActions)
        {
            if (action is SnakeActions.MoveForward)
            {
                control.Speed = MathF.Min(control.MaxSpeed, control.Speed + control.Acceleration * deltaTime);
            }
            else if (action is SnakeActions.SlowDown)
            {
                control.Speed = MathF.Max(0, control.Speed - control.Deceleration * deltaTime);
            }
            else if (action is SnakeActions.TurnLeft)
            {
                transform.Rotation -= control.MaxTurnRate * deltaTime;
            }
            else if (action is SnakeActions.TurnRight)
            {
                transform.Rotation += control.MaxTurnRate * deltaTime;
            }
            else if (action is SnakeActions.GenericAction)
            {
                _pendingAddSegmentCommands.Add(new SnakeCommands.AddSnakeSegmentCommand(
                    entityId
                ));
            }
        }

        // Update position based on speed and rotation
        float radians = MathF.PI / 180f * transform.Rotation;
        transform.Position += new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * control.Speed * deltaTime;

        // Clear pending actions after processing
        inputReceiver.PendingActions.Clear();
    }

    private void ProcessSnakeBody(IWorld world, float deltaTime, ref SnakeControl control, ref Transform2d transform)
    {
        // Start with the head
        Transform2d previousTransform = transform;

        // Iterate over each segment and update its position based on the previous segment
        for (int segmentIndex = 0; segmentIndex < control.BodySegments.Count; segmentIndex++)
        {
            var currentEntity = world.Entities.QueryById(control.BodySegments[segmentIndex]);
            ref var currentTransform = ref currentEntity.GetRef<Transform2d>();
            var diff = previousTransform.Position - currentTransform.Position;
            float distance = diff.Length();

            if (distance > control.SegmentSpacing)
            {
                var direction = Vector2.Normalize(diff);
                var newPosition = previousTransform.Position - direction * control.SegmentSpacing;
                currentTransform.Position = Vector2.Lerp(currentTransform.Position, newPosition, 10f * deltaTime);
                currentTransform.Rotation = MathF.Atan2(direction.Y, direction.X) * 180f / MathF.PI;
            }

            previousTransform = currentTransform;
        }
    }

    private void SpawnNewSnakeBodySegments(IWorld world)
    {
        foreach (var command in _pendingAddSegmentCommands)
        {
            var entity = world.Entities.QueryById(command.EntityId);
            ref var control = ref entity.GetRef<SnakeControl>();
            ref var transform = ref entity.GetRef<Transform2d>();
            AddBodySegment(world, ref control, ref transform);
        }

        _pendingAddSegmentCommands.Clear();
    }

    private void AddBodySegment(
        IWorld world,
        ref SnakeControl snake,
        ref Transform2d headTransform)
    {
        if (snake.BodySegments.Count == 0)
        {
            SpawnSegment(world, snake.BodySegments, headTransform);

            return;
        }

        var tailEntity = world.Entities.QueryById(snake.BodySegments[^1]);
        var tailTransform = tailEntity.GetCopy<Transform2d>();

        SpawnSegment(world, snake.BodySegments, tailTransform);
    }

    private void SpawnSegment(IWorld world, List<EntityId> segments, Transform2d transform)
    {
        EntityId segmentId = world.Entities.AddEntity(
            transform,
            new BasicShape(ShapeType.Circle, _segmentHalfSize, Color.DarkGreen)
        );

        segments.Add(segmentId);
    }
}
