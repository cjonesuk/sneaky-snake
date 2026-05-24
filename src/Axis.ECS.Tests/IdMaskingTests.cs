namespace Axis.ECS.Tests;

public class IdMaskingTests
{
    [Fact]
    public void GenerationMask_IsCorrect()
    {
        const uint expected = 0xFF00_0000;
        Assert.Equal(expected, IdSpace.GenerationMask);
    }

    [Fact]
    public void RoleMask_IsCorrect()
    {
        const ulong expected = 0xFF00_0000_0000_0000;
        Assert.Equal(expected, IdSpace.RoleMask);
    }

    [Fact]
    public void PairFlag_IsCorrect()
    {
        const ulong expected = 0x8000_0000_0000_0000;
        Assert.Equal(expected, IdSpace.PairFlag);
    }
}