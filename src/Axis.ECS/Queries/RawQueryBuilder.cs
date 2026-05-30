namespace Axis.ECS.Queries;

/// <summary>Raw terms-based query builder. The user-facing builders (<c>QueryBuilder</c> / <c>QueryBuilder&lt;T0..TN&gt;</c>) are generated wrappers around this.</summary>
public sealed class RawQueryBuilder
{
    private List<QueryTerm> _terms;
    private World _world;

    private RawQueryBuilder()
    {
        _terms = new List<QueryTerm>(8);
        _world = null!;
    }

    private void Setup(World world)
    {
        _world = world;
        _terms.Clear();
    }

    public static RawQueryBuilder For(World world)
    {
        var builder = new RawQueryBuilder();
        builder.Setup(world);
        return builder;
    }

    public RawQueryBuilder Add(QueryTerm term)
    {
        _terms.Add(term);
        return this;
    }

    public RawQueryBuilder Add<T>() where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        return Add(new QueryTerm(componentId, QueryTermKind.Include));
    }

    /// <summary>Filter the query to entities that have <typeparamref name="T"/>. Unlike <see cref="Add{T}"/>, the generated <c>QueryBuilder&lt;T0..TN&gt;</c> does not yield a <c>ref T</c> for this term. Useful for tags and other presence-only checks.</summary>
    public RawQueryBuilder With<T>() where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        return Add(new QueryTerm(componentId, QueryTermKind.Include));
    }

    public RawQueryBuilder Without<T>() where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        return Add(new QueryTerm(componentId, QueryTermKind.Exclude));
    }

    public ArchetypeQuery Build()
    {
        return new ArchetypeQuery(_world, _terms.ToArray());
    }
}
