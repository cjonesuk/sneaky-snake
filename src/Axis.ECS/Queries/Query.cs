using System.Runtime.InteropServices;

namespace Axis.ECS.Queries;


public sealed class ArchetypeQuery
{
    private readonly World _world;
    private readonly QueryTerm[] _terms;
    private readonly List<Archetype> _cachedResults;
    private ulong _cachedStructuralVersion;

    internal ArchetypeQuery(World world, QueryTerm[] terms)
    {
        _world = world;
        _terms = terms;
        _cachedResults = new List<Archetype>();
        _cachedStructuralVersion = 0;
    }

    internal ReadOnlySpan<Archetype> Run()
    {
        if (_cachedStructuralVersion != _world.Archetypes.StructuralVersion)
        {
            QueryAlgorithms.FindArchetypes(_world, _terms, _cachedResults);
            _cachedStructuralVersion = _world.Archetypes.StructuralVersion;
        }

        return CollectionsMarshal.AsSpan(_cachedResults);
    }

    /// <summary>Sum of entity counts across every matching archetype.</summary>
    public int Count()
    {
        int total = 0;
        foreach (var archetype in Run())
        {
            total += archetype.EntityCount;
        }
        return total;
    }
}