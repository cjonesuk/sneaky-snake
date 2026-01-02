using Shouldly;

namespace AnECS.Tests;

public class EntityTypeTests
{
    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsSubset()
    {
        var entityType1 = new EntityType(new int[] { 1, 2, 3, 4, 5 });
        var entityType2 = new EntityType(new int[] { 2, 4 });

        bool result = entityType1.HasSubset(entityType2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenOtherIsNotSubset()
    {
        var entityType1 = new EntityType(new int[] { 1, 2, 3, 4, 5 });
        var entityType2 = new EntityType(new int[] { 2, 6 });

        bool result = entityType1.HasSubset(entityType2);
        result.ShouldBeFalse();
    }

}
