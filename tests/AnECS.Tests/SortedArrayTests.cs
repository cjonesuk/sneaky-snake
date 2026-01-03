using Shouldly;

namespace AnECS.Tests;

public class SortedArrayTests
{
    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsSubset()
    {
        var array1 = SortedArray<int>.Create([1, 2, 3, 4, 5]);
        var array2 = SortedArray<int>.Create([2, 4]);

        bool result = array1.HasSubset(array2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenOtherIsNotSubset()
    {
        var array1 = SortedArray<int>.Create([1, 2, 3, 4, 5]);
        var array2 = SortedArray<int>.Create([2, 6]);

        bool result = array1.HasSubset(array2);

        result.ShouldBeFalse();
    }

    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsEmpty()
    {
        var array1 = SortedArray<int>.Create([1, 2, 3]);
        var array2 = SortedArray<int>.Create(Array.Empty<int>());

        bool result = array1.HasSubset(array2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenThisIsEmptyAndOtherIsNot()
    {
        var array1 = SortedArray<int>.Create(Array.Empty<int>());
        var array2 = SortedArray<int>.Create([1]);

        bool result = array1.HasSubset(array2);

        result.ShouldBeFalse();
    }

    [Fact]
    public void With_AddsNewItem_WhenItemNotPresent()
    {
        var array = SortedArray<int>.Create([1, 3, 5]);

        array.With(4, out var newArray).ShouldBe(true);
        newArray.AsSpan().ToArray().ShouldBe([1, 3, 4, 5]);
    }

    [Fact]
    public void With_DoesNotAddItem_WhenItemAlreadyPresent()
    {
        var array = SortedArray<int>.Create([1, 3, 5]);
        array.With(3, out var newArray).ShouldBe(false);
        newArray.AsSpan().ToArray().ShouldBe([1, 3, 5]);
    }

    [Fact]
    public void Without_RemovesItem_WhenItemIsPresent()
    {
        var array = SortedArray<int>.Create([1, 3, 5]);

        array.Without(3, out var newArray).ShouldBe(true);
        newArray.AsSpan().ToArray().ShouldBe([1, 5]);
    }

    [Fact]
    public void Without_DoesNotRemoveItem_WhenItemIsNotPresent()
    {
        var array = SortedArray<int>.Create([1, 3, 5]);
        array.Without(4, out var newArray).ShouldBe(false);
        newArray.AsSpan().ToArray().ShouldBe([1, 3, 5]);
    }

}