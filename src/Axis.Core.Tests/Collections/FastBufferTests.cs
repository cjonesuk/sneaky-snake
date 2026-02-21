using Shouldly;

namespace Axis.Core.Collections;

public class FastBufferTests
{
    [Fact]
    public void Test()
    {
        FastBuffer<int> buffer = FastBuffer<int>.Create(2);

        buffer.Add(1);
        buffer.Add(2);

        buffer.Count.ShouldBe(2);
        buffer.AsSpan().ToArray().ShouldBe([1, 2]);

        buffer.Add(3);

        buffer.Count.ShouldBe(3);
        buffer.AsSpan().ToArray().ShouldBe([1, 2, 3]);
    }
}