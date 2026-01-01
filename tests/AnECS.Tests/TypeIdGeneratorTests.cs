using Shouldly;

namespace AnECS.Tests;

public class TypeIdGeneratorTests
{
    [Fact]
    public void DifferentTypesHaveDifferentTypeIds()
    {
        int typeId1 = TypeIdGenerator.NextTypeId();
        int typeId2 = TypeIdGenerator.NextTypeId();

        typeId1.ShouldNotBe(typeId2);
    }

}
