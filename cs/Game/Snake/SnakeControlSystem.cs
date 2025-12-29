using System.Numerics;
using Engine.Components;
using Engine.WorldManagement;

namespace SneakySnake;

internal sealed class SnakeControlSystem : IWorldSystem
{
    private readonly float _friction = 25f;

    public void Update(IWorld world, float deltaTime)
    {
        ProcessSnakeHead(world, deltaTime);
        ProcessSnakeBody(world, deltaTime);
    }

    private void ProcessSnakeBody(IWorld world, float deltaTime)
    {
        var result = world.Entities.QueryAll<Transform2d, SnakeSegments>();

        foreach (var (_, transforms, segmentsList) in result)
        {
            for (int index = 0; index < transforms.Length; index++)
            {
                ref var segments = ref segmentsList[index];
                ref var transform = ref transforms[index];

                // Start with the head
                Transform2d previousTransform = transform;

                // Iterate over each segment and update its position based on the previous segment
                for (int segmentIndex = 0; segmentIndex < segments.Entities.Count; segmentIndex++)
                {
                    var currentEntity = world.Entities.QueryById(segments.Entities[segmentIndex]);
                    ref var currentTransform = ref currentEntity.GetRef<Transform2d>();
                    var diff = previousTransform.Position - currentTransform.Position;
                    float distance = diff.Length();

                    if (distance > segments.SegmentSpacing)
                    {
                        var direction = Vector2.Normalize(diff);
                        var newPosition = previousTransform.Position - direction * segments.SegmentSpacing;
                        currentTransform.Position = Vector2.Lerp(currentTransform.Position, newPosition, 10f * deltaTime);
                        currentTransform.Rotation = MathF.Atan2(direction.Y, direction.X) * 180f / MathF.PI;
                    }

                    previousTransform = currentTransform;
                }
            }
        }
    }

    private void ProcessSnakeHead(IWorld world, float deltaTime)
    {
        var result = world.Entities.QueryAll<Transform2d, SnakeControl, InputActionReceiver>();

        foreach (var (_, transforms, controls, inputReceivers) in result)
        {
            for (int index = 0; index < transforms.Length; index++)
            {
                ref var transform = ref transforms[index];
                ref var control = ref controls[index];
                ref var inputReceiver = ref inputReceivers[index];

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
                }

                // Update position based on speed and rotation
                float radians = MathF.PI / 180f * transform.Rotation;
                transform.Position += new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * control.Speed * deltaTime;

                // Clear pending actions after processing
                inputReceiver.PendingActions.Clear();

                transforms[index] = transform;
                controls[index] = control;
            }
        }
    }
}
