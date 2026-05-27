namespace Axis.ECS.Queries;

internal static class QueryAlgorithms
{
    public static void FindArchetypes(
        World world,
        ReadOnlySpan<QueryTerm> terms,
        List<Archetype> results)
    {
        var archetypes = world.Archetypes.GetAllArchetypes();

        results.Clear();

        for (int index = 0; index < archetypes.Length; index++)
        {
            var archetype = archetypes[index];

            if (MatchesAllTerms(archetype, terms))
            {
                results.Add(archetype);
            }
        }
    }

    private static bool MatchesAllTerms(Archetype archetype, ReadOnlySpan<QueryTerm> terms)
    {
        ReadOnlySpan<Id> componentIds = archetype.EntityType.ComponentIds;

        foreach (var term in terms)
        {
            bool contains = componentIds.Contains(term.ComponentId);

            switch (term.Kind)
            {
                case QueryTermKind.Include:
                    if (!contains) return false;
                    break;
                case QueryTermKind.Exclude:
                    if (contains) return false;
                    break;
            }
        }

        return true;
    }
}
