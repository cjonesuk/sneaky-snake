namespace Engine.WorldManagement.Entities;

internal readonly struct EntityCreationRequest
{
    public readonly EntityId EntityId;
    public readonly object[] Components;

    internal EntityCreationRequest(EntityId entityId, object[] components)
    {
        EntityId = entityId;
        Components = components;
    }
}
