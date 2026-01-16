using System.Runtime.InteropServices;

namespace Axis.ECS;

internal sealed class ArchetypeManager
{
    private readonly Archetype _emptyArchetype;
    private readonly List<Archetype> _archetypes;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;

    public ArchetypeManager()
    {
        _emptyArchetype = new Archetype(EntityType.Empty);
        _archetypes = new List<Archetype>() { _emptyArchetype };
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
    }

    public Archetype EmptyArchetype => _emptyArchetype;

    public Archetype GetOrCreate<T1>()
        where T1 : unmanaged
    {
        EntityType entityType = EntityTypeInformation<T1>.EntityType;
        return GetOrCreate(entityType);
    }

    public Archetype GetOrCreate<T1, T2>()
        where T1 : unmanaged
        where T2 : unmanaged
    {
        EntityType entityType = EntityTypeInformation<T1, T2>.EntityType;
        return GetOrCreate(entityType);
    }

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

    public ArchetypeQueryEnumerable QueryArchetypes()
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1> QueryArchetypes<T1>()
        where T1 : unmanaged
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1>(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1, T2> QueryArchetypes<T1, T2>()
        where T1 : unmanaged
        where T2 : unmanaged
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1, T2>(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1, T2, T3> QueryArchetypes<T1, T2, T3>()
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1, T2, T3>(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1, T2, T3, T4> QueryArchetypes<T1, T2, T3, T4>()
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1, T2, T3, T4>(archetypesSpan);
    }

    public ArchetypeQueryEnumerable<T1, T2, T3, T4, T5> QueryArchetypes<T1, T2, T3, T4, T5>()
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        Span<Archetype> archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeQueryEnumerable<T1, T2, T3, T4, T5>(archetypesSpan);
    }

    /// <summary>
    /// Clears all archetypes without removing them or resizing the underlying collections.
    /// </summary>
    public void ClearAll()
    {
        foreach (var archetype in _archetypes)
        {
            archetype.Clear();
        }
    }
}
