using Shouldly;

namespace Axis.ECS.Tests;

public class EntityTypeTests
{
    Id _id1 = Id.Make(1, 0);
    Id _id2 = Id.Make(2, 0);
    Id _id3 = Id.Make(3, 0);
    Id _id4 = Id.Make(4, 0);
    Id _id5 = Id.Make(5, 0);

    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsSubset()
    {
        var entityType1 = EntityType.Create([_id1, _id2, _id3, _id4, _id5]);
        var entityType2 = EntityType.Create([_id2, _id4]);

        bool result = entityType1.HasSubset(entityType2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenOtherIsNotSubset()
    {
        var entityType1 = EntityType.Create([_id1, _id2, _id3, _id4, _id5]);
        var entityType2 = EntityType.Create([_id2, Id.Make(6, 0)]);

        bool result = entityType1.HasSubset(entityType2);
        result.ShouldBeFalse();
    }

    [Fact]
    public void Create_WithDuplicates_Throws()
    {
        Should.Throw<InvalidOperationException>(() => EntityType.Create([_id1, _id2, _id3, _id2]));
    }

}
