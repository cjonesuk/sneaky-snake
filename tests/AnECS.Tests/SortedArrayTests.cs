using Shouldly;

namespace AnECS.Tests;

public class SortedArrayTests
{
    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsSubset()
    {
        var array1 = new SortedArray<int>(new int[] { 1, 2, 3, 4, 5 });
        var array2 = new SortedArray<int>(new int[] { 2, 4 });

        bool result = array1.HasSubset(array2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenOtherIsNotSubset()
    {
        var array1 = new SortedArray<int>(new int[] { 1, 2, 3, 4, 5 });
        var array2 = new SortedArray<int>(new int[] { 2, 6 });

        bool result = array1.HasSubset(array2);

        result.ShouldBeFalse();
    }

    [Fact]
    public void HasSubset_ReturnsTrue_WhenOtherIsEmpty()
    {
        var array1 = new SortedArray<int>(new int[] { 1, 2, 3 });
        var array2 = new SortedArray<int>(Array.Empty<int>());

        bool result = array1.HasSubset(array2);

        result.ShouldBeTrue();
    }

    [Fact]
    public void HasSubset_ReturnsFalse_WhenThisIsEmptyAndOtherIsNot()
    {
        var array1 = new SortedArray<int>(Array.Empty<int>());
        var array2 = new SortedArray<int>(new int[] { 1 });

        bool result = array1.HasSubset(array2);

        result.ShouldBeFalse();
    }

}