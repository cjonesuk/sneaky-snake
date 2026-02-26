using System.Diagnostics;
using Axis.ECS.Commands;
using Axis.ECS.Events;
using Axis.ECS.Queries;

namespace Axis.ECS;

public class WorldPairs(Id isA)
{
    public Id IsA { get; } = isA;
}

public struct IsA { };


public sealed class World : IWorld
{
    private readonly EntityIdManager _entityIds;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly ArchetypeManager _archetypes;
    private readonly ComponentEntityManager _components;
    private readonly WorldSystemScheduler _systemScheduler;
    private readonly EventManager _events;
    private readonly WorldCommandQueue _commands;
    private bool _deferredMode;
    private List<IArchetypeQuery> _activeQueries;
    private readonly WorldPairs _pairs;

    internal World()
    {
        _entityIds = new EntityIdManager();
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _components = new ComponentEntityManager(_entityIds);
        _archetypes = new ArchetypeManager(this, _components);
        _systemScheduler = new WorldSystemScheduler();
        _events = new EventManager();
        _commands = new WorldCommandQueue();
        _deferredMode = false;
        _activeQueries = new List<IArchetypeQuery>();
        _pairs = CreatePairs();
    }

    private WorldPairs CreatePairs()
    {
        Id isAId = _components.Register<IsA>();
        return new WorldPairs(isAId);
    }

    public static World Create()
    {
        return new World();
    }

    public WorldPairs Pairs => _pairs;
    internal IEventManager Events => _events;
    internal WorldSystemScheduler Systems => _systemScheduler;
    internal ComponentEntityManager Components => _components;

    internal ArchetypeManager Archetypes => _archetypes;


    internal void RegisterQuery(IArchetypeQuery query)
    {
        _activeQueries.Add(query);
    }

    public void RegisterComponent<T>() where T : unmanaged
    {
        Id id = _components.Register<T>();
        Console.WriteLine($"Registered component {typeof(T).Name} with Id {id}");

        CreateEntityWithId(id);
    }

    public WorldDeferredCommandsScope BeginDeferringCommands()
    {
        Debug.Assert(!_deferredMode, "World is already in deferred command mode.");

        _deferredMode = true;
        return new WorldDeferredCommandsScope(this);
    }

    public void EndDeferringCommands()
    {
        Debug.Assert(_deferredMode, "World is not in deferred command mode.");

        _deferredMode = false;
        _commands.ApplyAndClear(this);
    }

    internal void FlushCommands()
    {
        if (_deferredMode)
        {
            return;
        }

        _commands.ApplyAndClear(this);
    }

    public void ClearAllEvents()
    {
        _events.ClearAllEvents();
    }

    public void ExecuteSystems(float deltaTime)
    {
        var context = WorldSystemContext.For(this, deltaTime);
        var systems = _systemScheduler.GetSystems();

        foreach (var system in systems)
        {
            using var deferredMode = BeginDeferringCommands();
            system.Execute(context);
        }
    }


    private Id AllocateEntityId()
    {
        return _entityIds.AllocateEntityId();
    }

    public EntityBuilder DefineEntity()
    {
        Id id = AllocateEntityId();
        return new EntityBuilder(this, _commands, id);
    }

    public Entity CreateEntity()
    {
        Id id = AllocateEntityId();

        CreateEntityWithId(id);

        return Entity.Create(this, id);
    }

    internal void CreateEntityWithId(Id id)
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id);
            return;
        }

        AllocateEntity(EntityType.Empty, id);
    }

    internal EntityLocation AllocateEntity(EntityType entityType, Id id)
    {
        Archetype archetype = _archetypes.GetOrCreate(entityType);
        EntityLocation location = archetype.AllocateEntity(in id);
        _entityIndices[id] = location;
        return location;
    }

    public Entity GetEntity(Id id)
    {
        return Entity.Create(this, id);
    }

    public void RemoveEntity(Id id)
    {
        if (_deferredMode)
        {
            _commands.RemoveEntity(id);
            return;
        }

        EntityLocation location = FindEntity(id);
        location.Archetype.RemoveEntity(location.Index);
        _entityIndices.Remove(id);
    }

    /// <summary>
    /// Clears all entities and components from the world without removing archetypes or resizing underlying collections.
    /// </summary>
    public void RemoveAllEntities()
    {
        if (_deferredMode)
        {
            _commands.ClearAllEntities();
        }

        _archetypes.ClearAll();
        _entityIndices.Clear();
    }

    public void SetComponentOnEntity<T>(Id id, ref T component) where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        SetComponentOnEntity(id, componentId, ref component);
    }

    public void SetPairOnEntity<TRelationship>(Id id, Id target, ref TRelationship component) where TRelationship : unmanaged
    {
        Id relationshipId = _components.GetId<TRelationship>();
        Id componentId = Id.Pair(relationshipId, target);

        SetComponentOnEntity(id, componentId, ref component);
    }

    public void SetComponentOnEntity<T>(Id id, Id componentId, ref T component) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.SetComponent(id, componentId, ref component);
            return;
        }

        EntityLocation location = FindEntity(id);

        if (!location.Archetype.TrySetComponent(location.Index, componentId, ref component))
        {
            AddComponentToEntityInternal(id, componentId, ref component, location);
        }
    }

    public EntityLocation FindEntity(Id id)
    {
        if (!_entityIndices.TryGetValue(id, out EntityLocation location))
        {
            // todo: handle this scenario instead of failing
            throw new InvalidOperationException($"Entity with Id {id} does not exist in the world.");
        }

        return location;
    }

    public bool EntityHasComponent<T>(Id id) where T : unmanaged
    {
        EntityLocation location = FindEntity(id);

        return location.Archetype.SupportsComponentType<T>();
    }

    public void AddComponentToEntity<T>(Id id) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddComponent<T>(id);
            return;
        }

        EntityLocation location = FindEntity(id);
        Id componentId = _components.GetId<T>();
        T value = default;

        AddComponentToEntityInternal(id, componentId, ref value, location);
    }

    public void RemoveComponentFromEntity<T>(Id id) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.RemoveComponent<T>(ref id);
            return;
        }

        Id componentId = _components.GetId<T>();
        EntityLocation location = FindEntity(id);

        if (!location.Archetype.EntityType.Without(componentId, out var nextEntityType))
        {
            // Component not present, nothing to do.
            return;
        }

        Archetype nextArchetype = _archetypes.GetOrCreate(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntityDown(location);

        _entityIndices[id] = nextLocation;
    }

    public ref T GetComponentFromEntity<T>(Id id) where T : unmanaged
    {
        EntityLocation location = FindEntity(id);

        return ref location.Archetype.GetComponentRef<T>(location.Index);
    }

    public EntityType GetEntityType(Id id)
    {
        EntityLocation location = FindEntity(id);
        return location.Archetype.EntityType;
    }

    private void AddComponentToEntityInternal<T>(Id id, Id componentId, ref T component, EntityLocation location) where T : unmanaged
    {
        if (!location.Archetype.EntityType.With(componentId, out var nextEntityType))
        {
            throw new InvalidOperationException("Failed to extend entity: resulting EntityType is the same as the current one.");
        }

        Archetype nextArchetype = _archetypes.GetOrCreate(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntityUp(location, componentId, ref component);

        _entityIndices[id] = nextLocation;
    }

    public bool IsEntityAlive(Id id)
    {
        return _entityIndices.ContainsKey(id);
    }
}

