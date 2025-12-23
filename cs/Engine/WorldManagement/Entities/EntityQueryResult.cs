using System.Runtime.InteropServices;

namespace Engine.WorldManagement.Entities;


public readonly struct EntityQueryAllResult<T1, T2> : IEnumerable<(EntityListView<T1>, EntityListView<T2>)>
{
    private readonly List<Archetype> MatchingArchetypes;

    internal EntityQueryAllResult(List<Archetype> matchingArchetypes)
    {
        MatchingArchetypes = matchingArchetypes;
    }

    public IEnumerator<(EntityListView<T1>, EntityListView<T2>)> GetEnumerator()
    {
        foreach (var archetype in MatchingArchetypes)
        {
            var components1 = archetype.GetComponents<T1>();
            var components2 = archetype.GetComponents<T2>();

            yield return (new EntityListView<T1>(components1), new EntityListView<T2>(components2));
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}


public readonly struct EntityQueryResult
{
    private readonly EntityLocation _location;

    internal EntityQueryResult(EntityLocation location)
    {
        _location = location;
    }

    public T1 GetCopy<T1>()
    {
        return _location.Archetype.GetComponents<T1>()[_location.Index];
    }

    public ref T GetRef<T>()
    {
        var components = _location.Archetype.GetComponents<T>();
        var span = CollectionsMarshal.AsSpan(components);
        return ref span[_location.Index];
    }
}