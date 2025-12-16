namespace Engine;

internal readonly struct EntityRemovalRequest
{
    public readonly EntityId EntityId;

    internal EntityRemovalRequest(EntityId entityId)
    {
        EntityId = entityId;
    }
}
