using Engine.WorldManagement.Entities;

namespace SneakySnake;

internal struct SnakeControl
{
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float MaxTurnRate;
    public float Speed;
    public List<EntityId> BodySegments;
    public float SegmentSpacing;

    private SnakeControl(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate,
        float segmentSpacing,
        List<EntityId> bodySegments)
    {
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        Deceleration = deceleration;
        MaxTurnRate = maxTurnRate;
        SegmentSpacing = segmentSpacing;
        BodySegments = bodySegments;
        Speed = 0f;
    }

    public static SnakeControl Create(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate,
        float segmentSpacing)
    {
        return new SnakeControl(
            maxSpeed,
            acceleration,
            deceleration,
            maxTurnRate,
            segmentSpacing,
            new List<EntityId>());
    }


}
