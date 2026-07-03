using Axis.ECS.Events;

namespace Axis.ECS;

/// <summary>Container for entities, components, systems, events, and world data.</summary>
public interface IWorld
{
    /// <summary>Event streams for world-level and per-entity events.</summary>
    IEventManager Events { get; }

    /// <summary>Spawn a new entity with no components.</summary>
    Entity SpawnEntity();

    /// <summary>Wrap an existing entity Id in a handle. Does not validate liveness.</summary>
    Entity GetEntity(Id id);

    /// <summary>Remove every entity. Previously-set world data is lost.</summary>
    void RemoveAllEntities();

    /// <summary>Remove a single entity.</summary>
    void RemoveEntity(Id id);

    /// <summary>Get the component-set the entity currently belongs to.</summary>
    EntityType GetEntityType(Id id);

    /// <summary>True if the Id maps to a live entity. Stale Ids return false.</summary>
    bool IsEntityAlive(Id id);

    /// <summary>Clear every event stream.</summary>
    void ClearAllEvents();

    /// <summary>Run all registered systems in registration order.</summary>
    void ExecuteSystems(float deltaTime);

    /// <summary>Add the component if missing, otherwise overwrite its value.</summary>
    void EnsureComponentOnEntity<T>(Id id, ref T component) where T : unmanaged;

    /// <inheritdoc cref="EnsureComponentOnEntity{T}(Id, ref T)"/>
    void EnsureComponentOnEntity<T>(Id id, Id componentId, ref T component) where T : unmanaged;

    /// <summary>Attach a relationship pair to the entity with the relationship's data.</summary>
    void SetPairOnEntity<TRelationship>(Id id, Id target, ref TRelationship component) where TRelationship : unmanaged;

    /// <summary>Attach a default-valued component. Throws if already present.</summary>
    void AddComponentToEntity<T>(Id id) where T : unmanaged;

    /// <summary>True if the entity has a component of type <typeparamref name="T"/>.</summary>
    bool EntityHasComponent<T>(Id id) where T : unmanaged;

    /// <summary>Remove the component. No-op if not present.</summary>
    void RemoveComponentFromEntity<T>(Id id) where T : unmanaged;

    /// <summary>Get a mutable ref to the component. Throws if not present.</summary>
    ref T GetComponentFromEntity<T>(Id id) where T : unmanaged;

    /// <summary>Store a small unmanaged value on the world (game state, not assets).</summary>
    void SetData<T>(in T value) where T : unmanaged;

    /// <summary>Get a mutable ref to the world data. Throws if not set.</summary>
    ref T GetData<T>() where T : unmanaged;

    /// <summary>True if world data of type <typeparamref name="T"/> is set.</summary>
    bool HasData<T>() where T : unmanaged;

    /// <summary>Remove the world data. No-op if not set.</summary>
    void RemoveData<T>() where T : unmanaged;
}
