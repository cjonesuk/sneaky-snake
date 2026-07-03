using System.Runtime.InteropServices;

namespace Axis.ECS;


internal sealed class ArchetypeManager
{
    private readonly World _world;
    private readonly ComponentEntityManager _components;
    private readonly Archetype _emptyArchetype;
    private readonly List<Archetype> _archetypes;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;
    private ulong _structuralVersion;

    internal ArchetypeManager(World world, ComponentEntityManager components)
    {
        _world = world;
        _components = components;
        _emptyArchetype = new Archetype(_world, _components, EntityType.Empty);
        _archetypes = new List<Archetype>() { _emptyArchetype };
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
        _structuralVersion = 0;
    }

    internal Archetype EmptyArchetype => _emptyArchetype;
    public ulong StructuralVersion => _structuralVersion;

    internal Archetype GetOrCreate(EntityType entityType)
    {
        if (_archetypesByEntityType.TryGetValue(entityType, out Archetype? archetype))
        {
            return archetype;
        }

        archetype = new Archetype(_world, _components, entityType);
        _archetypesByEntityType[entityType] = archetype;
        _archetypes.Add(archetype);

        _structuralVersion++;

        return archetype;
    }

    internal ReadOnlySpan<Archetype> GetAllArchetypes()
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
