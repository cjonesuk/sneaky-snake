using System.Collections;
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

    void IArchetypeQuery.Invalidate()
    {
        _cachedResults.Clear();
        _isCacheValid = false;
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

public ref struct Iter : IEnumerable<int>
{
    private readonly Archetype _archetype;

    public Iter(Archetype archetype)
    {
        _archetype = archetype;
    }

    public IterEnumerator GetEnumerator()
    {
        return new IterEnumerator(_archetype.EntityCount);
    }

    IEnumerator<int> IEnumerable<int>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public struct IterEnumerator : IEnumerator<int>
{
    private readonly int _length;
    private int _currentIndex;

    internal IterEnumerator(int length)
    {
        _length = length;
        _currentIndex = -1;
    }

    public bool MoveNext()
    {
        return ++_currentIndex < _length;
    }

    public int Current => _currentIndex;

    int IEnumerator<int>.Current => Current;

    public void Reset()
    {
        _currentIndex = -1;
    }

    public void Dispose()
    {
    }

    object IEnumerator.Current => Current;

    bool IEnumerator.MoveNext()
    {
        return MoveNext();
    }

    void IEnumerator.Reset()
    {
        Reset();
    }

    void IDisposable.Dispose()
    {
        Dispose();
    }
}