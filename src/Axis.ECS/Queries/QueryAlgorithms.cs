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
            bool matches = true;

            foreach (var term in terms)
            {
                var contains = archetype.EntityType.ComponentIds.Contains(term.ComponentId);
                if (!contains)
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                results.Add(archetype);
            }
        }
    }
}

