namespace AnECS;

public delegate void EntityQueryAction<T1>(ref Id id, ref T1 arg1)
    where T1 : notnull;

public delegate void EntityQueryAction<T1, T2>(ref Id id, ref T1 arg1, ref T2 arg2)
    where T1 : notnull
    where T2 : notnull;

public interface IWorld
{
    void SetComponentOnEntity<T>(Id id, T component) where T : notnull;
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

    public void SetComponentOnEntity<T>(Id id, T component) where T : notnull
    {
        EntityLocation location = FindEntity(id);

        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();

        if (location.Archetype.SupportsComponentType(componentTypeId))
        {
            location.Archetype.SetComponent(location.Index, component);
            return;
        }

        ExtendEntity(id, component, location, componentTypeId);
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

    public void AddComponentToEntity<T>(Id id) where T : notnull
    {
        EntityLocation location = FindEntity(id);

        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();

        ExtendEntity(id, default(T), location, componentTypeId);
        
    }

    private void ExtendEntity<T>(Id id, T component, EntityLocation location, ComponentTypeId componentTypeId) where T : notnull
    {
        EntityType nextEntityType = location.Archetype.EntityType.WithAddedComponent(componentTypeId);

        if (nextEntityType.Equals(location.Archetype.EntityType))
        {
            throw new InvalidOperationException("Failed to extend entity: resulting EntityType is the same as the current one.");
        }

        Archetype nextArchetype = GetArchetype(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location, new EntityComponentValue(componentTypeId, component));

        _entityIndices[id] = nextLocation;
    }


    public void Query<T1>(EntityQueryAction<T1> action) where T1 : notnull
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
        where T1 : notnull
        where T2 : notnull
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
