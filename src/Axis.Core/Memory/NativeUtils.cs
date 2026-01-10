using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Axis.Core.Memory;

internal static class NativeUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static int AlignOf<T>() where T : unmanaged
    {
        return sizeof(AlignOfHelper<T>) - sizeof(T);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static int SizeOf<T>() where T : unmanaged
    {
        return sizeof(T);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AlignOfHelper<T> where T : unmanaged
    {
        public byte dummy;
        public T data;
    }
}