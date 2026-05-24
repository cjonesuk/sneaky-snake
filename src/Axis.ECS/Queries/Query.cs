using System.Runtime.InteropServices;

namespace Axis.ECS.Queries;


public interface IArchetypeQuery
{
    void Invalidate();
}

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
        world.RegisterQuery(this);
    }

    void IArchetypeQuery.Invalidate()
    {
        _cachedResults.Clear();
        _isCacheValid = false;
    }

    /// <summary>
    /// Stops this query from being invalidated when archetypes change. After calling,
    /// subsequent <see cref="Run"/> calls may return stale results. Idempotent.
    /// </summary>
    public void Unregister()
    {
        _world.UnregisterQuery(this);
    }

    internal ReadOnlySpan<Archetype> Run()
    {
        if (!_isCacheValid)
        {
            QueryAlgorithms.FindArchetypes(_world, _terms, _cachedResults);
            _isCacheValid = true;
        }

        return CollectionsMarshal.AsSpan(_cachedResults);
    }
}