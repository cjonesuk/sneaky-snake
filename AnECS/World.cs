namespace AnECS;

public delegate void EntityQueryAction<T1>(ref Id id, ref T1 arg1)
    where T1 : struct;

public delegate void EntityQueryAction<T1, T2>(ref Id id, ref T1 arg1, ref T2 arg2)
    where T1 : struct
    where T2 : struct;

public interface IWorld
{
    void SetComponentOnEntity<T>(Id id, T component) where T : struct;
    void AddComponentToEntity<T>(Id id) where T : struct;
    bool EntityHasComponent<T>(Id id) where T : struct;
    void RemoveComponentFromEntity<T>(Id id) where T : struct;
    ref T GetComponentFromEntity<T>(Id id) where T : struct;
}

internal sealed class World : IWorld
{
    private uint _nextId = 1;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;
    private readonly List<Archetype> _archetypes;
    private readonly Archetype _emptyArchetype;

    internal World()
    {
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _emptyArchetype = new Archetype(EntityType.Empty);
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
        _archetypes = new List<Archetype>() { };
    }

    public static World Create()
    {
        return new World();
    }

    public Entity Entity()
    {
        Id id = new Id(this, _nextId++);
        EntityLocation location = _emptyArchetype.AddEntity(id, Span<EntityComponentValue>.Empty);
        _entityIndices[id] = location;

        return new Entity(id);
    }

    public void SetComponentOnEntity<T>(Id id, T component) where T : struct
    {
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

    public bool EntityHasComponent<T>(Id id) where T : struct
    {
        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();

        EntityLocation location = FindEntity(id);

        return location.Archetype.SupportsComponentType(componentTypeId);
    }

    public void AddComponentToEntity<T>(Id id) where T : struct
    {
        EntityLocation location = FindEntity(id);

        AddComponentToEntityInternal<T>(id, default, location);
    }

    public void RemoveComponentFromEntity<T>(Id id) where T : struct
    {
        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();
        EntityLocation location = FindEntity(id);

        if (!location.Archetype.EntityType.Without(componentTypeId, out var nextEntityType))
        {
            // Component not present, nothing to do.
            return;
        }

        Archetype nextArchetype = GetArchetype(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location);

        _entityIndices[id] = nextLocation;
    }

    public ref T GetComponentFromEntity<T>(Id id) where T : struct
    {
        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();
        EntityLocation location = FindEntity(id);

        return ref location.Archetype.GetComponentRef<T>(location.Index);

    }

    private void AddComponentToEntityInternal<T>(Id id, T component, EntityLocation location) where T : struct
    {
        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();

        if (!location.Archetype.EntityType.With(componentTypeId, out var nextEntityType))
        {
            throw new InvalidOperationException("Failed to extend entity: resulting EntityType is the same as the current one.");
        }

        Archetype nextArchetype = GetArchetype(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location, new EntityComponentValue(componentTypeId, component));

        _entityIndices[id] = nextLocation;
    }


    public void Query<T1>(EntityQueryAction<T1> action) where T1 : struct
    {
        EntityType entityType = EntityTypeInformation<T1>.EntityType;

        foreach (var archetype in _archetypes)
        {
            if (!archetype.EntityType.HasSubset(entityType))
            {
                continue;
            }

            archetype.Query(action);
        }
    }

    public void Query<T1, T2>(EntityQueryAction<T1, T2> action)
        where T1 : struct
        where T2 : struct
    {
        EntityType entityType = EntityTypeInformation<T1, T2>.EntityType;

        foreach (var archetype in _archetypes)
        {
            if (!archetype.EntityType.HasSubset(entityType))
            {
                continue;
            }

            archetype.Query(action);
        }
    }

    private Archetype GetArchetype(EntityType entityType)
    {
        if (_archetypesByEntityType.TryGetValue(entityType, out Archetype? archetype))
        {
            return archetype;
        }

        archetype = new Archetype(entityType);
        _archetypesByEntityType[entityType] = archetype;
        _archetypes.Add(archetype);

        return archetype;
    }

}
