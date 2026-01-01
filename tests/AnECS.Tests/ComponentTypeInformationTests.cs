using Shouldly;

namespace AnECS.Tests;

public class ComponentTypeInformationTests
{
    [Fact]
    public void DifferentTypesHaveDifferentTypeIds()
    {
        int typeId1 = ComponentTypeInformation<int>.Id;
        int typeId2 = ComponentTypeInformation<string>.Id;

        typeId1.ShouldNotBe(typeId2);
    }

    [Fact]
    public void SameTypeHasSameTypeId()
    {
        int typeId1 = ComponentTypeInformation<double>.Id;
        int typeId2 = ComponentTypeInformation<double>.Id;
        typeId1.ShouldBe(typeId2);
    }

}