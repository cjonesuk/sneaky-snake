namespace AnECS;

internal sealed class ArchetypeManager
{
    private readonly Archetype _emptyArchetype;
    private readonly List<Archetype> _archetypes;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;

    public ArchetypeManager()
    {
        _emptyArchetype = new Archetype(EntityType.Empty);
        _archetypes = new List<Archetype>();
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
    }

    public Archetype EmptyArchetype => _emptyArchetype;

    public Archetype GetOrCreate(EntityType entityType)
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

    public ArchetypeEnumerable GetArchetypesWithComponents<T>() where T : struct
    {
        EntityType entityType = EntityTypeInformation<T>.EntityType;
        return new ArchetypeEnumerable(_archetypes, entityType);
    }

    public ArchetypeEnumerable GetArchetypesWithComponents<T1, T2>()
        where T1 : struct
        where T2 : struct
    {
        EntityType entityType = EntityTypeInformation<T1, T2>.EntityType;
        return new ArchetypeEnumerable(_archetypes, entityType);
    }
}
