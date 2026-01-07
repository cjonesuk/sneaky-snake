using System.Diagnostics;
using Axis.ECS.Commands;

namespace Axis.ECS;

public interface IWorld
{
    WorldSystemScheduler Systems { get; }

    Entity CreateEntity();
    void RemoveEntity(Id id);
    EntityType GetEntityType(Id id);
    bool IsEntityAlive(Id id);

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

internal sealed class World : IWorld
{
    private uint _nextId = 1;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly ArchetypeManager _archetypes;
    private readonly WorldSystemScheduler _systemScheduler;
    private CommandQueue _commands;
    private bool _deferredMode;

    internal World()
    {
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _archetypes = new ArchetypeManager();
        _systemScheduler = new WorldSystemScheduler();
        _commands = new CommandQueue();
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
        var data = new WorldSystemData(this, deltaTime);
        _systemScheduler.ExecuteAll(ref data);
    }

    private Id AllocateId()
    {
        return new Id(_nextId++);
    }

    public Entity CreateEntity()
    {
        Id id = AllocateId();

        CreateEntityWithId(id);

        return new Entity(this, id);
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

        return new Entity(this, id);
    }

    public void CreateEntityWithId<T1>(ref Id id, ref T1 c1) where T1 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(id, c1);
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

        return new Entity(this, id);
    }

    public void CreateEntityWithId<T1, T2>(ref Id id, ref T1 c1, ref T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        if (_deferredMode)
        {
            _commands.AddEntity(ref id, ref c1, ref c2);
            return;
        }

        Archetype archetype = _archetypes.GetOrCreate<T1, T2>();

        EntityLocation location = archetype.AddEntity(id, ref c1, ref c2);
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
    public void ClearAll()
    {
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
        EntityLocation location = FindEntity(id);

        AddComponentToEntityInternal<T>(id, default, location);
    }

    public void RemoveComponentFromEntity<T>(Id id) where T : unmanaged
    {
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


    public void QueryAll<T1>(QueryAllEntitiesAction<T1> action) where T1 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1>())
        {
            action(archetype.EntityIds, archetype.Col1);
        }
    }

    public void QueryAll<T1, T2>(QueryAllEntitiesAction<T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2>())
        {
            action(archetype.EntityIds, archetype.Col1, archetype.Col2);
        }
    }

    public void QueryEach(QueryEachEntityAction action)
    {
        foreach (var archetype in _archetypes.QueryArchetypes())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                action(ref archetype.EntityIds[index]);
            }
        }

    }

    public void QueryEach<T1>(QueryEachEntityAction<T1> action) where T1 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                action(ref archetype.EntityIds[index], ref archetype.Col1[index]);
            }
        }
    }

    public void QueryEach<T1, T2>(QueryEachEntityAction<T1, T2> action)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        foreach (var archetype in _archetypes.QueryArchetypes<T1, T2>())
        {
            for (int index = 0; index < archetype.EntityIds.Length; index++)
            {
                action(ref archetype.EntityIds[index],
                       ref archetype.Col1[index],
                       ref archetype.Col2[index]);
            }
        }
    }

    public bool IsEntityAlive(Id id)
    {
        return _entityIndices.ContainsKey(id);
    }
}

public static class WorldExtensions
{
    public static void AddSystem(this IWorld world, IWorldSystem system)
    {
        world.Systems.AddSystem(system);
    }

    public static SystemBuilder<T1> System<T1>(this IWorld world) where T1 : unmanaged
    {
        return new SystemBuilder<T1>(world);
    }

    public static SystemBuilder<T1, T2> System<T1, T2>(this IWorld world)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        return new SystemBuilder<T1, T2>(world);
    }
}