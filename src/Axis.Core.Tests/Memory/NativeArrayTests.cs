namespace Axis.Core.Memory.Tests;

using Shouldly;
using Xunit;


public unsafe class NativeBufferTests
{
    private readonly record struct TestStruct(int A, float B);

    [Fact]
    public void AllocAndReadBack_ShouldMatchOriginal()
    {
        using var buf = new NativeBuffer(128);
        var original = new TestStruct(42, 3.14f);
        int offset = buf.Alloc(original);

        offset.ShouldBeGreaterThanOrEqualTo(0);
        TestStruct* ptr = buf.GetPtr<TestStruct>(offset);
        ptr->A.ShouldBe(42);
        ptr->B.ShouldBe(3.14f, 0.0001);
    }

    [Fact]
    public void MultipleAllocations_ShouldBeContiguousAndAligned()
    {
        using var buf = new NativeBuffer(64);
        var a = new TestStruct(1, 1);
        var b = new TestStruct(2, 2);

        int offA = buf.Alloc(a);
        int offB = buf.Alloc(b);

        offB.ShouldBeGreaterThan(offA);

        int alignA = NativeType<TestStruct>.Alignment;
        (offA % alignA).ShouldBe(0);
        (offB % alignA).ShouldBe(0);
    }

    [Fact]
    public void EnsureCapacity_ShouldExpandAndKeepData()
    {
        using var buf = new NativeBuffer(16);
        var s = new TestStruct(5, 6);

        int off1 = buf.Alloc(s);
        int oldCap = buf.Capacity;

        // allocate enough to force a resize
        for (int i = 0; i < 50; i++)
            buf.Alloc(s);

        buf.Capacity.ShouldBeGreaterThan(oldCap);

        TestStruct* ptr = buf.GetPtr<TestStruct>(off1);
        ptr->A.ShouldBe(5);
        ptr->B.ShouldBe(6);
    }

    [Fact]
    public void Reset_ShouldReuseMemory()
    {
        using var buf = new NativeBuffer(64);
        var s = new TestStruct(7, 8);
        buf.Alloc(s);
        buf.Used.ShouldBeGreaterThan(0);
        buf.Reset();
        buf.Used.ShouldBe(0);
        // Allocates again from start
        int off = buf.Alloc(s);
        off.ShouldBe(0);
    }

    [Fact]
    public void Dispose_ShouldFreeMemory()
    {
        var buf = new NativeBuffer(32);
        buf.Dispose();
        ((nint)buf.Ptr).ShouldBe(0);
        buf.Capacity.ShouldBe(0);
    }
}
