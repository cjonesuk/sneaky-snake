namespace SneakySnake;

internal struct SnakeControl
{
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
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        Deceleration = deceleration;
        MaxTurnRate = maxTurnRate;
        Speed = 0f;
    }
}
