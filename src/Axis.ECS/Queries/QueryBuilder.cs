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

public static class DefineQuery
{
    public static QueryBuilder<T0> For<T0>(World world) where T0 : unmanaged
    {
        var builder = QueryBuilder.For(world);
        builder.Add<T0>();
        return new QueryBuilder<T0>(builder);
    }
}

public struct QueryBuilder<T0> where T0 : unmanaged
{
    private QueryBuilder _builder;

    internal QueryBuilder(QueryBuilder builder)
    {
        _builder = builder;
    }

    public Query<T0> Build()
    {
        var query = _builder.Build();
        return new Query<T0>(query);
    }
}