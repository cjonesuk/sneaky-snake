using Axis.ECS;
using Axis.ECS.Queries;
using BenchmarkDotNet.Attributes;

namespace Axis.ECS.Benchmarks;

[MemoryDiagnoser]
public class ForEachAllocationBenchmark
{
    private World _world = null!;
    private Query<Health, Healing> _query;
    private int _accumulator;

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
    public void Static_Lambda_NoCapture()
    {
        _query.ForEach(static (Entity entity, ref Health h, ref Healing g) =>
        {
            h.Value += g.Amount;
        });
    }

    [Benchmark]
    public void NonStatic_Lambda_CapturesThis()
    {
        _query.ForEach((Entity entity, ref Health h, ref Healing g) =>
        {
            _accumulator += h.Value;
        });
    }
}
