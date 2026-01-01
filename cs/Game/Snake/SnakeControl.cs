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
    public float FollowStrength;

    private SnakeControl(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate,
        float segmentSpacing,
        float followStrength,
        List<EntityId> bodySegments)
    {
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        Deceleration = deceleration;
        MaxTurnRate = maxTurnRate;
        SegmentSpacing = segmentSpacing;
        FollowStrength = followStrength;
        BodySegments = bodySegments;
        Speed = 0f;
    }

    public static SnakeControl Create(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate,
        float segmentSpacing,
        float followStrength)
    {
        return new SnakeControl(
            maxSpeed,
            acceleration,
            deceleration,
            maxTurnRate,
            segmentSpacing,
            followStrength,
            new List<EntityId>());
    }


}
