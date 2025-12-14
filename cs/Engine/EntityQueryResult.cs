namespace Engine;

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


public readonly struct EntityQueryResult<T1, T2>
{
    private readonly EntityLocation _location;

    internal EntityQueryResult(EntityLocation location)
    {
        _location = location;
    }

    public void Deconstruct(out EntityAccessor<T1> one, out EntityAccessor<T2> two)
    {
        one = new EntityAccessor<T1>(_location.Archetype.GetComponents<T1>(), _location.Index);
        two = new EntityAccessor<T2>(_location.Archetype.GetComponents<T2>(), _location.Index);
    }
}