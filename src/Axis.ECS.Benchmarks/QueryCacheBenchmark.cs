using Axis.ECS;
using Axis.ECS.Queries;
using BenchmarkDotNet.Attributes;

namespace Axis.ECS.Benchmarks;

[MemoryDiagnoser]
public class QueryCacheBenchmark
{
    private World _world = null!;
    private Query<Health> _warmQuery;

    [GlobalSetup]
    public void Setup()
    {
        _world = World.Create();

        // Five different archetypes so the query must scan and filter
        for (int i = 0; i < 1_000; i++) _world.DefineEntity().With(new Health(i)).Build();
        for (int i = 0; i < 1_000; i++) _world.DefineEntity().With(new Health(i)).With(new Healing(1)).Build();
        for (int i = 0; i < 1_000; i++) _world.DefineEntity().With(new Position(i, i)).Build();
        for (int i = 0; i < 1_000; i++) _world.DefineEntity().With(new Health(i)).With(new Armor(10)).Build();
        for (int i = 0; i < 1_000; i++) _world.DefineEntity().With(new Velocity(i, i)).Build();

        _warmQuery = DefineQuery.For<Health>(_world).Build();
        // Prime the cache
        _warmQuery.ForEach(static (Entity e, ref Health h) => { });
    }

    [Benchmark(Baseline = true)]
    public void Warm_CachedQuery()
    {
        _warmQuery.ForEach(static (Entity e, ref Health h) => { h.Value++; });
    }

    [Benchmark]
    public void Cold_BuildEachCall()
    {
        var query = DefineQuery.For<Health>(_world).Build();
        query.ForEach(static (Entity e, ref Health h) => { h.Value++; });
    }
}
