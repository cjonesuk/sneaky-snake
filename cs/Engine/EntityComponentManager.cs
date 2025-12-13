namespace Engine;

internal class EntityComponentManager : IEntityComponentManager
{
    private readonly Dictionary<ArchetypeSignature, Archetype> _archetypes = new();
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

        archetype.AddEntity(entityId, components);

        return entityId;
    }

    public EntityQueryResult<T1, T2> QueryAll<T1, T2>()
    {
        var componentTypes = new Type[] { typeof(T1), typeof(T2) };

        var archetypes = _archetypes.Where(x => x.Key.ContainsAll(componentTypes))
                                    .Select(x => x.Value)
                                    .ToList();

        return new EntityQueryResult<T1, T2>(archetypes);
    }
}
