using System.Numerics;
using Engine.Components;
using Engine.WorldManagement;

namespace SneakySnake;

internal sealed class SnakeControlSystem : IWorldSystem
{
    private readonly float _friction = 5f;

    public void Update(IWorld world, float deltaTime)
    {
        var result = world.Entities.QueryAll<Transform2d, SnakeControl>();

        foreach (var (transforms, controls) in result)
        {
            for (int index = 0; index < transforms.Count; index++)
            {
                var transform = transforms[index];
                var control = controls[index];

                control.Speed = MathF.Max(0, control.Speed - _friction * deltaTime);

                foreach (var action in control.PendingActions)
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
                control.PendingActions.Clear();

                transforms[index] = transform;
                controls[index] = control;
            }
        }

    }
}
