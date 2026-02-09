using System.Runtime.InteropServices;

namespace Axis.ECS;


internal sealed class ArchetypeManager
{
    private readonly ComponentEntityManager _components;
    private readonly Archetype _emptyArchetype;
    private readonly List<Archetype> _archetypes;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;

    internal ArchetypeManager(ComponentEntityManager components)
    {
        _components = components;
        _emptyArchetype = new Archetype(_components, EntityType.Empty);
        _archetypes = new List<Archetype>() { _emptyArchetype };
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
    }

    internal Archetype EmptyArchetype => _emptyArchetype;

    internal Archetype GetOrCreate(EntityType entityType)
    {
        if (_archetypesByEntityType.TryGetValue(entityType, out Archetype? archetype))
        {
            return archetype;
        }

        archetype = new Archetype(_components, entityType);
        _archetypesByEntityType[entityType] = archetype;
        _archetypes.Add(archetype);

        return archetype;
    }

    internal Span<Archetype> GetAllArchetypes()
    {
        return CollectionsMarshal.AsSpan(_archetypes);
    }

    /// <summary>
    /// Clears all archetypes without removing them or resizing the underlying collections.
    /// </summary>
    internal void ClearAll()
    {
        foreach (var archetype in _archetypes)
        {
            archetype.Clear();
        }
    }
}
