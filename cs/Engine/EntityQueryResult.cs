namespace Engine;

public struct EntityQueryResult<T1, T2> : IEnumerable<(EntityListView<T1>, EntityListView<T2>)>
{
    internal List<Archetype> MatchingArchetypes;

    internal EntityQueryResult(List<Archetype> matchingArchetypes)
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
