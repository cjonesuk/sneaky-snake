using Shouldly;

namespace AnECS.Tests;

public class EntityTypeTests
{
    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsSubset()
    {
        var entityType1 = new EntityType([new(1), new(2), new(3), new(4), new(5)]);
        var entityType2 = new EntityType([new(2), new(4)]);

        bool result = entityType1.HasSubset(entityType2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenOtherIsNotSubset()
    {
        var entityType1 = new EntityType([new(1), new(2), new(3), new(4), new(5)]);
        var entityType2 = new EntityType([new(2), new(6)]);

        bool result = entityType1.HasSubset(entityType2);
        result.ShouldBeFalse();
    }

}
