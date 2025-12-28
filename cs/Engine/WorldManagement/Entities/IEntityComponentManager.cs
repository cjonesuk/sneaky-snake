namespace Engine.WorldManagement.Entities;


public interface IEntityComponentManager
{
    /// <summary>
    /// Queues the creation of a new entity with the specified components.
    /// The entity will be created when ProcessPendingCommands is called.
    /// </summary>
    EntityId AddEntity(params object[] components);

    /// <summary>
    /// Queues the removal of the specified entity.
    /// The entity will be removed when ProcessPendingCommands is called.
    /// </summary>
    void RemoveEntity(EntityId entityId);

    /// <summary>
    /// Processes all pending entity creation and removal commands.
    /// </summary>
    void ProcessPendingCommands();

    EntityQueryResult QueryById(EntityId entityId);
    bool TryQueryById(EntityId entityId, out EntityQueryResult result);

    EntityQueryAllResult<T1, T2> QueryAll<T1, T2>();

    EntityQueryAllResultV2<T1, T2> QueryAllV2<T1, T2>();

}