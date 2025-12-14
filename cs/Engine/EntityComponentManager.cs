using SneakySnake;

namespace Engine;

internal struct EntityLocation
{
    public Archetype Archetype;

    /// <summary>
    /// Index of the entity within the archetype's component lists
    /// </summary>
    public int Index;

    internal EntityLocation(Archetype archetype, int index)
    {
        Archetype = archetype;
        Index = index;
    }
}

internal class EntityComponentManager : IEntityComponentManager
{
    private readonly Dictionary<ArchetypeSignature, Archetype> _archetypes = new();
    private readonly Dictionary<EntityId, EntityLocation> _entityLocations = new();
    private int _nextEntityId = 1;

    private EntityId GetNextEntityId()
    {
        int id = _nextEntityId++;
        return new EntityId(id, 0);
    }

    private ArchetypeSignature GetArchetypeSignature(params object[] components)
    {
        Type[] componentTypes = components.Select(c => c.GetType()).ToArray();
        return new ArchetypeSignature(componentTypes);
    }

    private Archetype GetOrCreateArchetype(ArchetypeSignature signature)
    {
        if (!_archetypes.TryGetValue(signature, out var archetype))
        {
            archetype = new Archetype(signature);
            _archetypes[signature] = archetype;
        }

        return archetype;
    }

    public EntityId AddEntity(params object[] components)
    {
        EntityId entityId = GetNextEntityId();
        ArchetypeSignature signature = GetArchetypeSignature(components);

        Archetype archetype = GetOrCreateArchetype(signature);

        EntityLocation location = archetype.AddEntity(entityId, components);

        _entityLocations[entityId] = location;

        return entityId;
    }


    public void RemoveAllEntities()
    {
        _archetypes.Clear();
        _entityLocations.Clear();
    }

    public EntityQueryAllResult<T1, T2> QueryAll<T1, T2>()
    {
        var componentTypes = new Type[] { typeof(T1), typeof(T2) };

        var archetypes = _archetypes.Values
            .Where(x => x.Signature.ContainsAll(componentTypes))
            .ToList();

        return new EntityQueryAllResult<T1, T2>(archetypes);
    }

    public EntityQueryResult<T1, T2> QueryById<T1, T2>(EntityId entityId)
    {
        var location = _entityLocations[entityId];
        return new EntityQueryResult<T1, T2>(location);
    }
}
