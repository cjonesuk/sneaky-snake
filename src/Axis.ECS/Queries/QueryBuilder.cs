namespace Axis.ECS.Queries;

public sealed class QueryBuilder
{
    private List<QueryTerm> _terms;
    private World _world;

    private QueryBuilder()
    {
        _terms = new List<QueryTerm>(8);
        _world = null!;
    }

    private void Setup(World world)
    {
        _world = world;
        _terms.Clear();
    }

    public static QueryBuilder For(World world)
    {
        var builder = new QueryBuilder();
        builder.Setup(world);
        return builder;
    }

    public QueryBuilder Add(QueryTerm term)
    {
        _terms.Add(term);
        return this;
    }

    public QueryBuilder Add<T>() where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        return Add(new QueryTerm(componentId));
    }

    public ArchetypeQuery Build()
    {
        return new ArchetypeQuery(_world, _terms.ToArray());
    }
}

