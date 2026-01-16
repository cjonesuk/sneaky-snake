using System.Diagnostics;
using Axis.ECS.Commands;

namespace Axis.ECS;

public sealed class World : IWorld
{
    private uint _nextId = 1;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly ArchetypeManager _archetypes;
    private readonly WorldSystemScheduler _systemScheduler;
    private WorldCommandQueue _commands;
    private bool _deferredMode;

    internal World()
    {
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _archetypes = new ArchetypeManager();
        _systemScheduler = new WorldSystemScheduler();
        _commands = new WorldCommandQueue();
        _deferredMode = false;
    }

    public static World Create()
    {
        return new World();
    }

    public WorldSystemScheduler Systems => _systemScheduler;

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

    public void ExecuteSystems(float deltaTime)
    {
        var data = new WorldSystemContext(this, deltaTime);
        var systems = _systemScheduler.GetSystems();

        foreach (var system in systems)
        {
            using var deferredMode = BeginDeferringCommands();
            system.Execute(ref data);
        }
    }

    private Id AllocateId()
    {
        return new Id(_nextId++);
    }

    public Entity CreateEntity()
    {
        Id id = AllocateId();

        CreateEntityWithId(id);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId(Id id)
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id);
            return;
        }

        Archetype archetype = _archetypes.EmptyArchetype;
        EntityLocation location = archetype.AddEntity(id);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1>(T1 c1) where T1 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1>(ref Id id, ref T1 c1) where T1 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1>();

        EntityLocation location = archetype.AddEntity(id, ref c1);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2>(T1 c1, T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2>(ref Id id, ref T1 c1, ref T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3>(T1 c1, T2 c2, T3 c3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3, T4>(T1 c1, T2 c2, T3 c3, T4 c4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3, ref c4);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3, T4>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3, ref c4);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3, T4>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3, ref c4);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3, T4, T5>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3, ref c4, ref c5);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3, T4, T5>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3, T4, T5>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3, T4, T5, T6>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3, T4, T5, T6>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3, T4, T5, T6>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3, T4, T5, T6, T7>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3, T4, T5, T6, T7>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3, T4, T5, T6, T7>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7);
        _entityIndices[id] = location;
    }

    public Entity CreateEntity<T1, T2, T3, T4, T5, T6, T7, T8>(T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged
    {
        Id id = AllocateId();

        CreateEntityWithId(ref id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8);

        return Entity.Create(this, id);
    }

    public void CreateEntityWithId<T1, T2, T3, T4, T5, T6, T7, T8>(ref Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7, ref T8 c8)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2, T3, T4, T5, T6, T7, T8>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7, ref c8);
        _entityIndices[id] = location;
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

    public void SetComponentOnEntity<T>(Id id, T component) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.SetComponent(ref id, ref component);
            return;
        }

        EntityLocation location = FindEntity(id);

        if (location.Archetype.SupportsComponentType<T>())
        {
            location.Archetype.SetComponent(location.Index, component);
            return;
        }

        AddComponentToEntityInternal(id, component, location);
    }

    private EntityLocation FindEntity(Id id)
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

        AddComponentToEntityInternal<T>(id, default, location);
    }

    public void RemoveComponentFromEntity<T>(Id id) where T : unmanaged
    {
        if (_deferredMode)
        {
            _commands.RemoveComponent<T>(ref id);
            return;
        }

        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;
        EntityLocation location = FindEntity(id);

        if (!location.Archetype.EntityType.Without(componentTypeId, out var nextEntityType))
        {
            // Component not present, nothing to do.
            return;
        }

        Archetype nextArchetype = _archetypes.GetOrCreate(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location);

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

    private void AddComponentToEntityInternal<T>(Id id, T component, EntityLocation location) where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;

        if (!location.Archetype.EntityType.With(componentTypeId, out var nextEntityType))
        {
            throw new InvalidOperationException("Failed to extend entity: resulting EntityType is the same as the current one.");
        }

        Archetype nextArchetype = _archetypes.GetOrCreate(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location, ref component);

        _entityIndices[id] = nextLocation;
    }


    public void QueryAll<TContext, T1>(ref TContext context, QueryAllEntitiesAction<TContext, T1> action) where T1 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1>())
        {
            action(ref context, archetype.EntityIds, archetype.Col1);
        }
    }

    public void QueryAll<TContext, T1, T2>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2>())
        {
            action(ref context, archetype.EntityIds, archetype.Col1, archetype.Col2);
        }
    }

    public void QueryAll<TContext, T1, T2, T3>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3>())
        {
            action(ref context, archetype.EntityIds, archetype.Col1, archetype.Col2, archetype.Col3);
        }
    }

    public void QueryAll<TContext, T1, T2, T3, T4>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3, T4> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3, T4>())
        {
            action(ref context, archetype.EntityIds, archetype.Col1, archetype.Col2, archetype.Col3, archetype.Col4);
        }
    }

    public void QueryAll<TContext, T1, T2, T3, T4, T5>(ref TContext context, QueryAllEntitiesAction<TContext, T1, T2, T3, T4, T5> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3, T4, T5>())
        {
            action(ref context, archetype.EntityIds, archetype.Col1, archetype.Col2, archetype.Col3, archetype.Col4, archetype.Col5);
        }
    }

    public void QueryEach<TContext>(ref TContext context, QueryEachEntityAction<TContext> action)
    {
        foreach (var archetype in _archetypes.QueryArchetypes())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context, ref iter);
            }
        }
    }

    public void QueryEach<TContext, T1>(ref TContext context, QueryEachEntityAction<TContext, T1> action) where T1 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context, ref iter, ref archetype.Col1[index]);
            }
        }
    }

    public void QueryEach<TContext, T1, T2>(ref TContext context, QueryEachEntityAction<TContext, T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context,
                       ref iter,
                       ref archetype.Col1[index],
                       ref archetype.Col2[index]);
            }
        }
    }

    public void QueryEach<TContext, T1, T2, T3>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context,
                       ref iter,
                       ref archetype.Col1[index],
                       ref archetype.Col2[index],
                       ref archetype.Col3[index]);
            }
        }
    }

    public void QueryEach<TContext, T1, T2, T3, T4>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3, T4> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3, T4>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context,
                       ref iter,
                       ref archetype.Col1[index],
                       ref archetype.Col2[index],
                       ref archetype.Col3[index],
                       ref archetype.Col4[index]);
            }
        }
    }

    public void QueryEach<TContext, T1, T2, T3, T4, T5>(ref TContext context, QueryEachEntityAction<TContext, T1, T2, T3, T4, T5> action)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2, T3, T4, T5>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                Iter iter = new Iter(archetype.Archetype, archetype.EntityIds, index);
                action(ref context,
                       ref iter,
                       ref archetype.Col1[index],
                       ref archetype.Col2[index],
                       ref archetype.Col3[index],
                       ref archetype.Col4[index],
                       ref archetype.Col5[index]);
            }
        }
    }

    public bool IsEntityAlive(Id id)
    {
        return _entityIndices.ContainsKey(id);
    }
}
