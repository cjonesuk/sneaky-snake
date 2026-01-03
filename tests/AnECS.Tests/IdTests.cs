namespace AnECS.Tests;

using AnECS;
using Shouldly;


public class IdTests
{
    [Fact]
    public void EmptyIdIsInvalidId()
    {
        Id id = new Id();
        id.Value.ShouldBe((ulong)0);
        id.ShouldBe(Id.Invalid);
    }

    [Fact]
    public void IdEquality()
    {
        var world = World.Create();

        Id id1 = new Id(world, 42);
        Id id2 = new Id(world, 42);

        id1.ShouldBe(id2);
        (id1 == id2).ShouldBeTrue();
        (id1 != id2).ShouldBeFalse();
        id1.GetHashCode().ShouldBe(id2.GetHashCode());
    }

    [Fact]
    public void IdInequality()
    {
        var world = World.Create();

        Id id1 = new Id(world, 42);
        Id id2 = new Id(world, 43);
        id1.ShouldNotBe(id2);
        (id1 != id2).ShouldBeTrue();
        (id1 == id2).ShouldBeFalse();
        id1.GetHashCode().ShouldNotBe(id2.GetHashCode());
    }
}
