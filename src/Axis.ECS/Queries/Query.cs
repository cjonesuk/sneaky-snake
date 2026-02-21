using System.Runtime.InteropServices;

namespace Axis.ECS.Queries;


public interface IArchetypeQuery
{
    void Invalidate();
}

/// <summary>
///  
/// </summary>
public sealed class ArchetypeQuery : IArchetypeQuery
{
    private readonly World _world;
    private readonly QueryTerm[] _terms;
    private readonly List<Archetype> _cachedResults;
    private bool _isCacheValid;

    internal ArchetypeQuery(World world, QueryTerm[] terms)
    {
        _world = world;
        _terms = terms;
        _cachedResults = new List<Archetype>();
        _isCacheValid = false;
    }

    public ArchetypeEnumerable Run()
    {
        var results = GetResults();
        return new ArchetypeEnumerable(results);
    }

    void IArchetypeQuery.Invalidate()
    {
        _cachedResults.Clear();
        _isCacheValid = false;
    }

    private ReadOnlySpan<Archetype> GetResults()
    {
        if (!_isCacheValid)
        {
            QueryAlgorithms.FindArchetypes(_world, _terms, _cachedResults);
            _isCacheValid = true;
        }

        return CollectionsMarshal.AsSpan(_cachedResults);
    }
}

