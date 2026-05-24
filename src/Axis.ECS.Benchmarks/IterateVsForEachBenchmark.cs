using Axis.ECS;
using Axis.ECS.Queries;
using BenchmarkDotNet.Attributes;

namespace Axis.ECS.Benchmarks;

[MemoryDiagnoser]
public class IterateVsForEachBenchmark
{
    private World _world = null!;
    private Query<Health, Healing> _query;

    [GlobalSetup]
    public void Setup()
    {
        _world = World.Create();

        for (int i = 0; i < 10_000; i++)
        {
            _world.DefineEntity()
                .With(new Health(i))
                .With(new Healing(1))
                .Build();
        }

        _query = DefineQuery.For<Health, Healing>(_world).Build();
    }

    [Benchmark(Baseline = true)]
    public void Iterate_SpanAccess()
    {
        _query.Iterate(static (Span<Id> ids, Span<Health> healths, Span<Healing> healings) =>
        {
            for (int i = 0; i < ids.Length; i++)
            {
                healths[i].Value += healings[i].Amount;
            }
        });
    }

    [Benchmark]
    public void ForEach_PerEntity()
    {
        _query.ForEach(static (Entity entity, ref Health h, ref Healing g) =>
        {
            h.Value += g.Amount;
        });
    }
}
