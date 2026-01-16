namespace Axis.ECS;

public interface IWorld
{
    WorldSystemScheduler Systems { get; }

    Entity CreateEntity();
    Entity CreateEntity<T1>(T1 c1) where T1 : unmanaged;
    Entity CreateEntity<T1, T2>(T1 c1, T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged;

    Entity CreateEntity<T1, T2, T3>(T1 c1, T2 c2, T3 c3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged;

    Entity CreateEntity<T1, T2, T3, T4>(T1 c1, T2 c2, T3 c3, T4 c4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged;

    Entity CreateEntity<T1, T2, T3, T4, T5>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged;

    Entity CreateEntity<T1, T2, T3, T4, T5, T6>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged;

    Entity CreateEntity<T1, T2, T3, T4, T5, T6, T7>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged;

    Entity CreateEntity<T1, T2, T3, T4, T5, T6, T7, T8>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged;

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

    void QueryAll<TContext, T1, T2, T3>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged;

    void QueryAll<TContext, T1, T2, T3, T4>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3, T4> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged;

    void QueryAll<TContext, T1, T2, T3, T4, T5>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3, T4, T5> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged;

    void QueryEach<TContext, T1>(ref TContext context, QueryEachEntityAction<TContext, T1> action)
         where T1 : unmanaged;
    void QueryEach<TContext, T1, T2>(ref TContext context, QueryEachEntityAction<TContext, T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged;

    void QueryEach<TContext, T1, T2, T3>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged;

    void QueryEach<TContext, T1, T2, T3, T4>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3, T4> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged;

    void QueryEach<TContext, T1, T2, T3, T4, T5>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3, T4, T5> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged;
}
