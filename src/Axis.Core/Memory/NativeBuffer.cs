using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Axis.Core.Memory;

/// <summary>
/// Stores raw bytes in unmanaged memory, ensuring proper alignment
/// for each allocation.
/// </summary>
public unsafe sealed class NativeBuffer : IDisposable
{
    private byte* _ptr;
    private int _capacity;
    private int _used;

    public int Used => _used;
    public int Capacity => _capacity;
    public byte* Ptr => _ptr; // stable unmanaged pointer

    public NativeBuffer(int initialCapacity = 4096)
    {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity));

        _capacity = initialCapacity;
        _ptr = (byte*)NativeMemory.Alloc((nuint)_capacity);
        _used = 0;
    }

    private void EnsureCapacity(int bytes)
    {
        int needed = _used + bytes;
        if (needed <= _capacity)
        {
            return;
        }

        int newCap = _capacity;
        while (newCap < needed)
        {
            newCap *= 2;
        }

        _ptr = (byte*)NativeMemory.Realloc(_ptr, (nuint)newCap);
        _capacity = newCap;
    }

    /// <summary>
    /// Allocates space for a struct and copies its bytes into the arena.
    /// The allocation will be aligned for the type's natural alignment.
    /// Returns byte offset from the beginning of the arena.
    /// </summary>
    public int Alloc<T>(in T value) where T : unmanaged
    {
        int size = NativeType<T>.Size;
        int alignedUsed = NativeType<T>.AlignUp(_used);
        int newUsed = alignedUsed + size;

        EnsureCapacity(newUsed - _used);

        void* dest = _ptr + alignedUsed;
        Buffer.MemoryCopy(Unsafe.AsPointer(ref Unsafe.AsRef(in value)), dest, size, size);

        _used = newUsed;
        return alignedUsed;
    }

    /// <summary>
    /// Returns a pointer to the object stored at offset.
    /// </summary>
    public T* GetPtr<T>(int offset) where T : unmanaged
        => (T*)(_ptr + offset);

    /// <summary>
    /// Resets the arena (keeps the memory).
    /// </summary>
    public void Reset() => _used = 0;

    public void Dispose()
    {
        if (_ptr != null)
        {
            NativeMemory.Free(_ptr);
            _ptr = null;
            _capacity = 0;
            _used = 0;
        }
    }
}
