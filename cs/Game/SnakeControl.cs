using Engine.Input;

namespace SneakySnake;

internal struct SnakeControl
{
    public readonly List<InputAction> PendingActions;
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float MaxTurnRate;
    public float Speed;

    public SnakeControl(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate)
    {
        PendingActions = new List<InputAction>();
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        Deceleration = deceleration;
        MaxTurnRate = maxTurnRate;
        Speed = 0f;
    }
}
