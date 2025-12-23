using Engine.Input;
using Engine.WorldManagement.Entities;

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

internal struct SnakeSegments
{
    public List<EntityId> Entities;
    public float SegmentSpacing;

    public SnakeSegments(float segmentSpacing)
    {
        Entities = new List<EntityId>();
        SegmentSpacing = segmentSpacing;
    }
}