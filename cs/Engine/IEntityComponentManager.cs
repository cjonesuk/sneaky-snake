using System.ComponentModel;

namespace Engine;

public struct EntityQueryResult<T1, T2> : IEnumerable<(T1, T2)>
{
    internal List<Archetype> MatchingArchetypes;

    internal EntityQueryResult(List<Archetype> matchingArchetypes)
    {
        MatchingArchetypes = matchingArchetypes;
    }

    public IEnumerator<(T1, T2)> GetEnumerator()
    {
        foreach (var archetype in MatchingArchetypes)
        {
            var components1 = archetype.GetComponents<T1>();
            var components2 = archetype.GetComponents<T2>();

            for (int index = 0; index < components1.Count; index++)
            {
                yield return (components1[index], components2[index]);
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public interface IEntityComponentManager
{
    EntityId AddEntity(params object[] components);

    EntityQueryResult<T1, T2> QueryAll<T1, T2>();

}
