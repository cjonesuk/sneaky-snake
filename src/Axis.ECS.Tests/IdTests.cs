namespace Axis.ECS.Tests;

using Axis.ECS;
using Shouldly;


public class IdTests
{
    [Fact]
    public void EmptyIdIsInvalidId()
    {
        Id id = Id.Make(0, 0);
        id.Value.ShouldBe((ulong)0);
        id.ShouldBe(Id.Invalid);

        id.IsValid().ShouldBeFalse();
        id.IsPair().ShouldBeFalse();
    }

    [Fact]
    public void IdEquality()
    {
        Id id1 = Id.Make(42, 0);
        Id id2 = Id.Make(42, 0);

        id1.ShouldBe(id2);
        (id1 == id2).ShouldBeTrue();
        (id1 != id2).ShouldBeFalse();
        id1.GetHashCode().ShouldBe(id2.GetHashCode());
    }

    [Fact]
    public void IdWithHigherGenerationIsDifferent()
    {
        Id id1 = Id.Make(42, 0);
        Id id2 = Id.Make(42, 1);
        id1.ShouldNotBe(id2);
        (id1 != id2).ShouldBeTrue();
        (id1 == id2).ShouldBeFalse();
        id1.GetHashCode().ShouldNotBe(id2.GetHashCode());
    }

    [Fact]
    public void IdInequality()
    {
        Id id1 = Id.Make(42, 0);
        Id id2 = Id.Make(43, 0);
        id1.ShouldNotBe(id2);
        (id1 != id2).ShouldBeTrue();
        (id1 == id2).ShouldBeFalse();
        id1.GetHashCode().ShouldNotBe(id2.GetHashCode());
    }

    [Fact]
    public void PairIdCreation()
    {
        Id left = Id.Make(1, 0);
        Id right = Id.Make(2, 0);
        Id pair = Id.Pair(left, right);

        pair.IsPair().ShouldBeTrue();
        pair.IsValid().ShouldBeTrue();
    }

    [Fact]
    public void RelationshipAndTargetCanBeExtractedFromPair()
    {
        Id left = Id.Make(17, 0);
        Id right = Id.Make(52, 0);
        Id pair = Id.Pair(left, right);

        ulong relationship = IdSpace.Relationship(pair.Value);
        ulong target = IdSpace.Target(pair.Value);

        relationship.ShouldBe(17ul);
        target.ShouldBe(52ul);
    }
}
