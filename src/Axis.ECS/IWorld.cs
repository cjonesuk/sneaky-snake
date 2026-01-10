namespace Axis.ECS;

public interface IWorld
{
    WorldSystemScheduler Systems { get; }

    Entity CreateEntity();
    void RemoveEntity(Id id);
    EntityType GetEntityType(Id id);
    bool IsEntityAlive(Id id);

    void ExecuteSystems(float deltaTime);

    void SetComponentOnEntity<T>(Id id, T component) where T : unmanaged;
    void AddComponentToEntity<T>(Id id) where T : unmanaged;
    bool EntityHasComponent<T>(Id id) where T : unmanaged;
    void RemoveComponentFromEntity<T>(Id id) where T : unmanaged;
    ref T GetComponentFromEntity<T>(Id id) where T : unmanaged;

    void QueryAll<T1>(QueryAllEntitiesAction<T1> action) where T1 : unmanaged;
    void QueryAll<T1, T2>(QueryAllEntitiesAction<T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged;

    void QueryEach<T1>(QueryEachEntityAction<T1> action)
         where T1 : unmanaged;
    void QueryEach<T1, T2>(QueryEachEntityAction<T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged;
}
