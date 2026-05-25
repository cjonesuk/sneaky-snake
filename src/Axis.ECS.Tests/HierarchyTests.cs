using Shouldly;

namespace Axis.ECS.Tests;

public class HierarchyTests
{
    [Fact]
    public void SetParent_ThenGetParent_RoundTrips()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var child = world.SpawnEntity();

        child.SetParent(parent);

        child.GetParent().Id.ShouldBe(parent.Id);
    }

    [Fact]
    public void RemoveParent_ClearsParent()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var child = world.SpawnEntity();
        child.SetParent(parent);

        child.RemoveParent();

        child.HasParent().ShouldBeFalse();
        child.TryGetParent(out _).ShouldBeFalse();
    }

    [Fact]
    public void HasParent_TracksLifecycle()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var child = world.SpawnEntity();

        child.HasParent().ShouldBeFalse();
        child.SetParent(parent);
        child.HasParent().ShouldBeTrue();
        child.RemoveParent();
        child.HasParent().ShouldBeFalse();
    }

    [Fact]
    public void TryGetParent_OnOrphanedChild_ReturnsFalse()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var child = world.SpawnEntity();
        child.SetParent(parent);

        world.RemoveEntity(parent.Id);

        child.TryGetParent(out _).ShouldBeFalse();
        // The Parent component is still on the child — only the parent's liveness changed.
        child.HasParent().ShouldBeTrue();
    }

    [Fact]
    public void GetParent_OnUnparentedEntity_Throws()
    {
        var world = World.Create();
        var entity = world.SpawnEntity();

        Should.Throw<InvalidOperationException>(() => entity.GetParent());
    }

    [Fact]
    public void GetChildren_ReturnsAllChildrenOfParent()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var a = world.SpawnEntity();
        var b = world.SpawnEntity();
        var c = world.SpawnEntity();
        a.SetParent(parent);
        b.SetParent(parent);
        c.SetParent(parent);

        var seen = new List<Id>();
        foreach (var child in world.GetChildren(parent.Id))
        {
            seen.Add(child.Id);
        }

        seen.ShouldBe([a.Id, b.Id, c.Id], ignoreOrder: true);
    }

    [Fact]
    public void GetChildren_OnEntityWithNoChildren_YieldsNothing()
    {
        var world = World.Create();
        var entity = world.SpawnEntity();

        int count = 0;
        foreach (var _ in world.GetChildren(entity.Id))
        {
            count++;
        }

        count.ShouldBe(0);
    }

    [Fact]
    public void GetChildren_OnlyYieldsDirectChildren_NotGrandchildren()
    {
        var world = World.Create();
        var root = world.SpawnEntity();
        var mid = world.SpawnEntity();
        var leaf = world.SpawnEntity();
        mid.SetParent(root);
        leaf.SetParent(mid);

        var rootChildren = new List<Id>();
        foreach (var child in world.GetChildren(root.Id)) rootChildren.Add(child.Id);

        rootChildren.ShouldBe([mid.Id]);
    }

    [Fact]
    public void SetParent_TwiceUpdatesRatherThanThrows()
    {
        var world = World.Create();
        var p1 = world.SpawnEntity();
        var p2 = world.SpawnEntity();
        var child = world.SpawnEntity();

        child.SetParent(p1);
        child.SetParent(p2);

        child.GetParent().Id.ShouldBe(p2.Id);
    }

    [Fact]
    public void MultipleLevelsOfNesting_EachLevelReadsItsParent()
    {
        var world = World.Create();
        var a = world.SpawnEntity();
        var b = world.SpawnEntity();
        var c = world.SpawnEntity();
        b.SetParent(a);
        c.SetParent(b);

        b.GetParent().Id.ShouldBe(a.Id);
        c.GetParent().Id.ShouldBe(b.Id);
    }

    [Fact]
    public void RemoveEntityRecursive_DeletesParentAndAllDescendants()
    {
        var world = World.Create();
        var a = world.SpawnEntity();
        var b = world.SpawnEntity();
        var c = world.SpawnEntity();
        b.SetParent(a);
        c.SetParent(b);

        world.RemoveEntityRecursive(a.Id);

        world.IsEntityAlive(a.Id).ShouldBeFalse();
        world.IsEntityAlive(b.Id).ShouldBeFalse();
        world.IsEntityAlive(c.Id).ShouldBeFalse();
    }

    [Fact]
    public void RemoveEntity_DoesNotCascade()
    {
        var world = World.Create();
        var parent = world.SpawnEntity();
        var child = world.SpawnEntity();
        child.SetParent(parent);

        world.RemoveEntity(parent.Id);

        world.IsEntityAlive(parent.Id).ShouldBeFalse();
        world.IsEntityAlive(child.Id).ShouldBeTrue();
    }
}
