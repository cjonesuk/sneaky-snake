using Engine.WorldManagement.Entities;

namespace SneakySnake;

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