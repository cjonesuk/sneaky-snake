using Axis.ECS.Events;

namespace Axis.ECS;

public interface IWorld
{
    IEventManager Events { get; }

    Entity CreateEntity();

    Entity GetEntity(Id id);

    void RemoveAllEntities();
    void RemoveEntity(Id id);
    EntityType GetEntityType(Id id);
    bool IsEntityAlive(Id id);

    void ClearAllEvents();
    void ExecuteSystems(float deltaTime);

    void SetComponentOnEntity<T>(Id id, ref T component) where T : unmanaged;
    void SetComponentOnEntity<T>(Id id, Id componentId, ref T component) where T : unmanaged;
    void SetPairOnEntity<TRelationship>(Id id, Id target, ref TRelationship component) where TRelationship : unmanaged;

    void AddComponentToEntity<T>(Id id) where T : unmanaged;
    bool EntityHasComponent<T>(Id id) where T : unmanaged;
    void RemoveComponentFromEntity<T>(Id id) where T : unmanaged;
    ref T GetComponentFromEntity<T>(Id id) where T : unmanaged;


}
