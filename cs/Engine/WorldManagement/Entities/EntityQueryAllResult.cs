namespace Engine.WorldManagement.Entities;

public readonly struct EntityQueryAllResult<T1, T2> : IEnumerable<(EntityListView<EntityId>, EntityListView<T1>, EntityListView<T2>)>
{
    private readonly List<Archetype> MatchingArchetypes;

    internal EntityQueryAllResult(List<Archetype> matchingArchetypes)
    {
        MatchingArchetypes = matchingArchetypes;
    }

    public IEnumerator<(EntityListView<EntityId>, EntityListView<T1>, EntityListView<T2>)> GetEnumerator()
    {
        foreach (var archetype in MatchingArchetypes)
        {
            var entityIds = archetype.GetEntityIds();
            var components1 = archetype.GetComponents<T1>();
            var components2 = archetype.GetComponents<T2>();

            yield return (
                new EntityListView<EntityId>(entityIds),
                new EntityListView<T1>(components1),
                new EntityListView<T2>(components2)
            );
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
