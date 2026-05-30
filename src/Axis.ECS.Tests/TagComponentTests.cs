using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

public class TagComponentTests
{
    internal struct LocalEmptyTag { }
    internal struct LocalDataComponent { public int Value; }

    [Fact]
    public void StructWithNoFieldsIsDetectedAsTag()
    {
        ComponentTypeInformation<LocalEmptyTag>.IsTag.ShouldBeTrue();
    }

    [Fact]
    public void StructWithFieldsIsNotATag()
    {
        ComponentTypeInformation<LocalDataComponent>.IsTag.ShouldBeFalse();
    }

    [Fact]
    public void TagCanBeAddedAndDetectedViaHas()
    {
        var world = World.Create();
        var entity = world.SpawnEntity();
        entity.Add<LocalEmptyTag>();

        entity.Has<LocalEmptyTag>().ShouldBeTrue();
    }

    [Fact]
    public void ReadingTagViaGetRefThrowsInvalidOperationException()
    {
        var world = World.Create();
        var entity = world.SpawnEntity();
        entity.Add<LocalEmptyTag>();

        var ex = Should.Throw<InvalidOperationException>(() => entity.GetRef<LocalEmptyTag>());
        ex.Message.ShouldContain("tag");
        ex.Message.ShouldContain(nameof(LocalEmptyTag));
    }

    [Fact]
    public void Arity0QueryWithTagIteratesAllTaggedEntitiesExactlyOnce()
    {
        var world = World.Create();

        var e1 = world.SpawnEntity();
        e1.Add<LocalEmptyTag>();

        var e2 = world.SpawnEntity();
        e2.Set(new LocalDataComponent { Value = 5 });
        e2.Add<LocalEmptyTag>();

        var e3 = world.SpawnEntity();
        e3.Set(new LocalDataComponent { Value = 7 });
        // no tag

        var query = DefineQuery.For(world).With<LocalEmptyTag>().Build();

        var visited = new List<Id>();
        query.ForEach((Entity e) => visited.Add(e.Id));

        visited.Count.ShouldBe(2);
        visited.ShouldContain(e1.Id);
        visited.ShouldContain(e2.Id);
        visited.ShouldNotContain(e3.Id);
    }

    [Fact]
    public void CountSumsAcrossAllMatchingArchetypes()
    {
        var world = World.Create();

        // archetype A: tag only
        for (int i = 0; i < 3; i++)
        {
            world.SpawnEntity().Add<LocalEmptyTag>();
        }

        // archetype B: tag + data
        for (int i = 0; i < 4; i++)
        {
            var e = world.SpawnEntity();
            e.Set(new LocalDataComponent { Value = i });
            e.Add<LocalEmptyTag>();
        }

        // archetype C: data only (excluded)
        for (int i = 0; i < 5; i++)
        {
            world.SpawnEntity().Set(new LocalDataComponent { Value = i });
        }

        var query = DefineQuery.For(world).With<LocalEmptyTag>().Build();
        query.Count().ShouldBe(7);
    }

    [Fact]
    public void RemovingTagRemovesFromCount()
    {
        var world = World.Create();
        var entity = world.SpawnEntity();
        entity.Add<LocalEmptyTag>();

        var query = DefineQuery.For(world).With<LocalEmptyTag>().Build();
        query.Count().ShouldBe(1);

        entity.Remove<LocalEmptyTag>();
        query.Count().ShouldBe(0);
    }
}
