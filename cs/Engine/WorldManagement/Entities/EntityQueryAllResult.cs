using System.Runtime.InteropServices;

namespace Engine.WorldManagement.Entities;

[Obsolete("Use EntityQueryAllResultV2 instead")]
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


public readonly struct EntityQueryAllResultV2<T1, T2>
{
    private readonly List<Archetype> _archetypes;

    internal EntityQueryAllResultV2(List<Archetype> matchingArchetypes)
    {
        _archetypes = matchingArchetypes;
    }

    public int PageCount => _archetypes.Count;

    public Page GetPage(int pageIndex)
    {
        var archetype = _archetypes[pageIndex];
        return new Page(archetype);
    }

    public ref struct Page
    {
        public readonly Span<EntityId> EntityIds;
        public readonly Span<T1> Components1;
        public readonly Span<T2> Components2;

        internal Page(Archetype archetype)
        {
            EntityIds = CollectionsMarshal.AsSpan(archetype.GetEntityIds());
            Components1 = CollectionsMarshal.AsSpan(archetype.GetComponents<T1>());
            Components2 = CollectionsMarshal.AsSpan(archetype.GetComponents<T2>());
        }

        public void Deconstruct(out Span<EntityId> entityIds, out Span<T1> components1, out Span<T2> components2)
        {
            entityIds = EntityIds;
            components1 = Components1;
            components2 = Components2;
        }
    }
}
