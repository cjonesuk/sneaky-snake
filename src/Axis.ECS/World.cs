using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    private readonly ArchetypeQuery _parentQuery;
    private Entity _dataEntity;

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
        _parentQuery = QueryBuilder.For(this).Add<Parent>().Build();
        _dataEntity = SpawnEntity();
    }

    internal ArchetypeQuery ParentQuery => _parentQuery;

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
    public IEventManager Events => _events;
    internal WorldSystemScheduler Systems => _systemScheduler;
    internal ComponentEntityManager Components => _components;

    internal ArchetypeManager Archetypes => _archetypes;


    internal void RegisterQuery(IArchetypeQuery query)
    {
        _activeQueries.Add(query);
    }

    internal void UnregisterQuery(IArchetypeQuery query)
    {
        _activeQueries.Remove(query);
    }

    public void UnregisterAllQueries()
    {
        _activeQueries.Clear();
    }

    internal void InvalidateActiveQueries()
    {
        foreach (var query in _activeQueries)
        {
            query.Invalidate();
        }
    }

    public void RegisterComponent<T>() where T : unmanaged
    {
        Id id = _components.Register<T>();
        SpawnEntityWithId(id);
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
            system.Execute(ref context);
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

    public Entity SpawnEntity()
    {
        Id id = AllocateEntityId();

        SpawnEntityWithId(id);

        return Entity.Create(this, id);
    }

    internal void SpawnEntityWithId(Id id)
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
        EntityRef displaced = location.Archetype.RemoveEntity(location.Index);
        _entityIndices.Remove(id);
        RecordDisplaced(displaced);
        _entityIds.Free(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RecordDisplaced(EntityRef displaced)
    {
        if (!displaced.Exists) return;
        _entityIndices[displaced.Id] = displaced.Location;
    }

    /// <summary>
    /// Clears all entities and components from the world without removing archetypes or resizing underlying collections.
    /// </summary>
    public void RemoveAllEntities()
    {
        if (_deferredMode)
        {
            _commands.ClearAllEntities();
            return;
        }

        foreach (var id in _entityIndices.Keys)
        {
            _entityIds.Free(id);
        }

        _archetypes.ClearAll();
        _entityIndices.Clear();
        _dataEntity = SpawnEntity();
    }

    public void SetData<T>(in T value) where T : unmanaged
    {
        T copy = value;
        Id componentId = _components.GetId<T>();
        EnsureComponentOnEntity(_dataEntity.Id, componentId, ref copy);
    }

    public ref T GetData<T>() where T : unmanaged
    {
        if (!HasData<T>())
        {
            throw new InvalidOperationException($"World data of type {typeof(T).Name} has not been set.");
        }
        return ref GetComponentFromEntity<T>(_dataEntity.Id);
    }

    public bool HasData<T>() where T : unmanaged
    {
        return EntityHasComponent<T>(_dataEntity.Id);
    }

    public void RemoveData<T>() where T : unmanaged
    {
        RemoveComponentFromEntity<T>(_dataEntity.Id);
    }

    public void EnsureComponentOnEntity<T>(Id id, ref T component) where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        EnsureComponentOnEntity(id, componentId, ref component);
    }

    public void SetPairOnEntity<TRelationship>(Id id, Id target, ref TRelationship component) where TRelationship : unmanaged
    {
        Id relationshipId = _components.GetId<TRelationship>();
        Id componentId = Id.Pair(relationshipId, target);

        EnsureComponentOnEntity(id, componentId, ref component);
    }

    public void EnsureComponentOnEntity<T>(Id id, Id componentId, ref T component) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.SetComponent(id, componentId, ref component);
            return;
        }

        EntityLocation location = FindEntity(id);

        if (!location.Archetype.TrySetComponent(location.Index, componentId, in component))
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
            _commands.RemoveComponent<T>(id);
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

        (EntityLocation migratedTo, EntityRef displaced) = nextArchetype.MigrateEntityDown(location);

        _entityIndices[id] = migratedTo;
        RecordDisplaced(displaced);
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

        (EntityLocation migratedTo, EntityRef displaced) = nextArchetype.MigrateEntityUp(location, componentId, ref component);

        _entityIndices[id] = migratedTo;
        RecordDisplaced(displaced);
    }

    public bool IsEntityAlive(Id id)
    {
        return _entityIndices.ContainsKey(id);
    }
}

