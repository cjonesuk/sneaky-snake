using Axis.ECS;
using BenchmarkDotNet.Attributes;

namespace Axis.ECS.Benchmarks;

[MemoryDiagnoser]
public class ArchetypeMigrationBenchmark
{
    private const int EntityCount = 1_000;

    private World _world = null!;
    private Entity[] _entities = null!;

    [IterationSetup(Target = nameof(AddComponent_FromOne))]
    public void Setup_AddComponent_FromOne()
    {
        _world = World.Create();
        // Pre-create the destination archetype so we measure steady-state migration
        // rather than archetype-creation cost.
        _world.DefineEntity().With(new Position(0, 0)).With(new Health(0)).Build();

        _entities = new Entity[EntityCount];
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.DefineEntity().With(new Position(i, i)).Build();
        }
    }

    [Benchmark(OperationsPerInvoke = EntityCount)]
    public void AddComponent_FromOne()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i].Add<Health>();
        }
    }

    [IterationSetup(Target = nameof(AddComponent_FromFour))]
    public void Setup_AddComponent_FromFour()
    {
        _world = World.Create();
        _world.DefineEntity()
            .With(new Position(0, 0))
            .With(new Velocity(0, 0))
            .With(new Healing(0))
            .With(new Armor(0))
            .With(new Health(0))
            .Build();

        _entities = new Entity[EntityCount];
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.DefineEntity()
                .With(new Position(i, i))
                .With(new Velocity(1, 1))
                .With(new Healing(5))
                .With(new Armor(10))
                .Build();
        }
    }

    [Benchmark(OperationsPerInvoke = EntityCount)]
    public void AddComponent_FromFour()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i].Add<Health>();
        }
    }

    [IterationSetup(Target = nameof(RemoveComponent_FromFive))]
    public void Setup_RemoveComponent_FromFive()
    {
        _world = World.Create();
        // Pre-create the destination archetype.
        _world.DefineEntity()
            .With(new Position(0, 0))
            .With(new Velocity(0, 0))
            .With(new Healing(0))
            .With(new Armor(0))
            .Build();

        _entities = new Entity[EntityCount];
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.DefineEntity()
                .With(new Position(i, i))
                .With(new Velocity(1, 1))
                .With(new Healing(5))
                .With(new Armor(10))
                .With(new Health(100))
                .Build();
        }
    }

    [Benchmark(OperationsPerInvoke = EntityCount)]
    public void RemoveComponent_FromFive()
    {
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i].Remove<Health>();
        }
    }
}
