using System.Runtime.InteropServices;

namespace Axis.ECS.Queries;


internal interface IArchetypeQuery
{

}

public enum QueryTermBinding
{
    In,
    Out,
    InOut
}

public record struct QueryTerm(Id ComponentId, QueryTermBinding Binding);

public struct QueryBuilder
{
    private readonly World _world;
    private readonly List<QueryTerm> _terms;

    internal QueryBuilder(World world)
    {
        _world = world;
        _terms = new List<QueryTerm>();
    }

    public QueryBuilder AddTerm(QueryTerm term)
    {
        _terms.Add(term);
        return this;
    }

    public QueryBuilder Add<T>(QueryTermBinding binding = QueryTermBinding.In) where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        AddTerm(new QueryTerm(componentId, binding));
        return this;
    }

    public Query Build()
    {
        var query = new Query(_world, _terms.ToArray());
        _world.RegisterQuery(query);
        return query;
    }
}

/// <summary>
///  
/// </summary>
public sealed class Query : IArchetypeQuery
{
    /// QueryContainer needs to support:
    /// - Storing archetypes that match the query
    /// - Storing the filter criteria
    /// - Iterating over matching entities

    private readonly World _world;
    private readonly List<Archetype> _archetypes;
    private readonly QueryTerm[] _terms;
    private bool _valid;

    internal Query(World world, QueryTerm[] terms)
    {
        _world = world;
        _archetypes = new List<Archetype>();
        _terms = terms;
        _valid = false;
    }

    public ArchetypeEnumerable Run()
    {
        if (!_valid)
        {
            RefreshArchetypes();
        }

        var archetypesSpan = CollectionsMarshal.AsSpan(_archetypes);
        return new ArchetypeEnumerable(archetypesSpan);
    }

    private void RefreshArchetypes()
    {
        _valid = true;
        _archetypes.Clear();

        foreach (var archetype in _world.Archetypes.GetAllArchetypes())
        {
            foreach (var term in _terms)
            {
                if (!archetype.EntityType.ComponentIds.Contains(term.ComponentId))
                {
                    continue;
                }
            }

            _archetypes.Add(archetype);
        }
    }
}

