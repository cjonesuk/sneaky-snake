using System.Runtime.InteropServices;

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

    public ArchetypeQueryEnumerable<T1> QueryArchetypes<T1>()
        where T1 : struct
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1>(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1, T2> QueryArchetypes<T1, T2>()
        where T1 : struct
        where T2 : struct
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1, T2>(archetypesSpan);
    }
}
