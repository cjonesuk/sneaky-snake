namespace Engine.WorldManagement.Entities;

internal interface IEntityCommandBuffer
{
    (List<EntityRemovalRequest> removals, List<EntityCreationRequest> creations) GetPendingCommands();

    void AddEntity(EntityId entityId, object[] components);
    void RemoveEntity(EntityId entityId);

    void ClearPendingCommands();
}

internal sealed class EntityCommandBuffer : IEntityCommandBuffer
{
    private readonly List<EntityRemovalRequest> _entitiesToRemove = new();
    private readonly List<EntityCreationRequest> _entitiesToCreate = new();

    public (List<EntityRemovalRequest> removals, List<EntityCreationRequest> creations) GetPendingCommands()
    {
        return (_entitiesToRemove, _entitiesToCreate);
    }

    public void AddEntity(EntityId entityId, object[] components)
    {
        _entitiesToCreate.Add(new EntityCreationRequest(entityId, components));
    }

    public void RemoveEntity(EntityId entityId)
    {
        _entitiesToRemove.Add(new EntityRemovalRequest(entityId));
    }

    public void ClearPendingCommands()
    {
        _entitiesToRemove.Clear();
        _entitiesToCreate.Clear();
    }
}
