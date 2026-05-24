using Axis.ECS;
using BenchmarkDotNet.Attributes;

namespace Axis.ECS.Benchmarks;

[MemoryDiagnoser]
public class EntityCreationBenchmark
{
    private const int Count = 1_000;

    private World _world = null!;

    [IterationSetup]
    public void IterationSetup()
    {
        _world = World.Create();
    }

    [Benchmark(OperationsPerInvoke = Count)]
    public void Spawn_Empty()
    {
        for (int i = 0; i < Count; i++)
        {
            _world.SpawnEntity();
        }
    }

    [Benchmark(OperationsPerInvoke = Count)]
    public void Define_OneComponent()
    {
        for (int i = 0; i < Count; i++)
        {
            _world.DefineEntity()
                .With(new Health(100))
                .Build();
        }
    }

    [Benchmark(OperationsPerInvoke = Count)]
    public void Define_FourComponents()
    {
        for (int i = 0; i < Count; i++)
        {
            _world.DefineEntity()
                .With(new Health(100))
                .With(new Healing(10))
                .With(new Position(0, 0))
                .With(new Velocity(1, 1))
                .Build();
        }
    }

    [Benchmark(OperationsPerInvoke = Count)]
    public void Define_EightComponents()
    {
        for (int i = 0; i < Count; i++)
        {
            _world.DefineEntity()
                .With(new Health(100))
                .With(new Healing(10))
                .With(new Position(0, 0))
                .With(new Velocity(1, 1))
                .With(new Armor(50))
                .With(new Mana(100))
                .With(new Stamina(75))
                .With(new PlayerTag())
                .Build();
        }
    }
}
