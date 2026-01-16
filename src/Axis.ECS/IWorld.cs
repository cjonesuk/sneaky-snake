namespace Axis.ECS;

public interface IWorld
{
    WorldSystemScheduler Systems { get; }

    Entity CreateEntity();
    Entity CreateEntity<T1>(T1 c1) where T1 : unmanaged;
    Entity CreateEntity<T1, T2>(T1 c1, T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged;

    void RemoveAllEntities();
    void RemoveEntity(Id id);
    EntityType GetEntityType(Id id);
    bool IsEntityAlive(Id id);

    void ExecuteSystems(float deltaTime);

    void SetComponentOnEntity<T>(Id id, T component) where T : unmanaged;
    void AddComponentToEntity<T>(Id id) where T : unmanaged;
    bool EntityHasComponent<T>(Id id) where T : unmanaged;
    void RemoveComponentFromEntity<T>(Id id) where T : unmanaged;
    ref T GetComponentFromEntity<T>(Id id) where T : unmanaged;

    void QueryAll<TContext, T1>(ref TContext context, QueryAllEntitiesAction<TContext, T1> action) where T1 : unmanaged;
    void QueryAll<TContext, T1, T2>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged;

    void QueryEach<TContext, T1>(ref TContext context, QueryEachEntityAction<TContext, T1> action)
         where T1 : unmanaged;
    void QueryEach<TContext, T1, T2>(ref TContext context, QueryEachEntityAction<TContext, T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged;
}
