using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public class MigrationTests
{
    private record struct Pos(float X);
    private record struct Vel(float DX);
    private record struct Tgt(float V);

    [Fact]
    public void RemovingComponent_DoesNotLeaveStaleValueInSourceArchetypeColumn()
    {
        var world = World.Create();

        var a = world.SpawnEntity();
        a.Set(new Pos(1f));
        a.Set(new Tgt(42f));

        a.GetRef<Tgt>().V.ShouldBe(42f);

        a.Remove<Tgt>();

        var b = world.SpawnEntity();
        b.Set(new Pos(2f));
        b.Set(new Tgt(7f));

        b.GetRef<Tgt>().V.ShouldBe(7f);
    }

    [Fact]
    public void DeferredSet_OnNewEntity_DoesNotInheritPreviousEntitysValue()
    {
        var world = World.Create();

        var a = world.SpawnEntity();
        a.Set(new Pos(1f));
        a.Set(new Tgt(42f));

        using (world.BeginDeferringCommands())
        {
            a.Remove<Tgt>();
        }

        var b = world.SpawnEntity();
        b.Set(new Pos(2f));

        using (world.BeginDeferringCommands())
        {
            b.Set(new Tgt(7f));
        }

        b.GetRef<Tgt>().V.ShouldBe(7f);
    }

    [Fact]
    public void MigratingThroughThreeArchetypes_PreservesIndependentValues()
    {
        var world = World.Create();

        var a = world.SpawnEntity();
        a.Set(new Pos(1f));
        a.Set(new Vel(10f));
        a.Set(new Tgt(99f));

        a.Remove<Tgt>();

        var b = world.SpawnEntity();
        b.Set(new Pos(2f));
        b.Set(new Vel(20f));
        b.Set(new Tgt(123f));

        a.GetRef<Pos>().X.ShouldBe(1f);
        a.GetRef<Vel>().DX.ShouldBe(10f);
        b.GetRef<Pos>().X.ShouldBe(2f);
        b.GetRef<Vel>().DX.ShouldBe(20f);
        b.GetRef<Tgt>().V.ShouldBe(123f);
    }

    [Fact]
    public void QueryAfterRemoveAndAdd_SeesCorrectValuesAcrossEntities()
    {
        var world = World.Create();

        var a = world.SpawnEntity();
        a.Set(new Pos(1f));
        a.Set(new Tgt(42f));
        a.Remove<Tgt>();

        var b = world.SpawnEntity();
        b.Set(new Pos(2f));
        b.Set(new Tgt(7f));

        var query = DefineQuery.For<Tgt>(world).Build();
        var seen = new List<(Id Id, float V)>();
        query.ForEach((Entity entity, ref Tgt t) =>
        {
            seen.Add((entity.Id, t.V));
        });

        seen.Count.ShouldBe(1);
        seen[0].Id.ShouldBe(b.Id);
        seen[0].V.ShouldBe(7f);
    }
}
