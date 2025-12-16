namespace Engine;

internal interface IEntityCommandBuffer
{
    (List<EntityRemovalRequest> removals, List<EntityCreationRequest> creations, WorldResetRequest? worldReset) GetPendingCommands();

    /// <summary>
    /// Sets a pending request to create a new world, clearing all existing entities.
    /// </summary>
    void NewWorld();


    void AddEntity(EntityId entityId, object[] components);
    void RemoveEntity(EntityId entityId);

    void ClearPendingCommands();
}

internal sealed class EntityCommandBuffer : IEntityCommandBuffer
{
    private readonly List<EntityRemovalRequest> _entitiesToRemove = new();
    private readonly List<EntityCreationRequest> _entitiesToCreate = new();
    private WorldResetRequest? _worldResetRequest = null;

    public (List<EntityRemovalRequest> removals, List<EntityCreationRequest> creations, WorldResetRequest? worldReset) GetPendingCommands()
    {
        return (_entitiesToRemove, _entitiesToCreate, _worldResetRequest);
    }

    public void NewWorld()
    {
        _worldResetRequest = new WorldResetRequest();
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
        _worldResetRequest = null;
    }
}
