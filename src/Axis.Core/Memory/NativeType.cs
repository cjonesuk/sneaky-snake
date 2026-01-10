using System.Runtime.CompilerServices;

namespace Axis.Core.Memory;

public static class NativeType<T> where T : unmanaged
{
    public static readonly int Size = NativeUtils.SizeOf<T>();
    public static readonly int Alignment = NativeUtils.AlignOf<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AlignUp(int value)
    {
        return (value + (Alignment - 1)) & ~(Alignment - 1);
    }
}
