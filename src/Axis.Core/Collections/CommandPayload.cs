using System.Runtime.CompilerServices;

namespace Axis.Core.Collections;

public unsafe readonly struct CommandPayload(void* payloadPtr)
{
    private readonly void* _payloadPtr = payloadPtr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CommandPayload From(void* basePtr, int offset)
    {
        void* payloadPtr = (byte*)basePtr + offset;
        return new CommandPayload(payloadPtr);
    }

    public void* Ptr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _payloadPtr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TPayload GetRef<TPayload>()
        where TPayload : unmanaged
    {
        return ref Unsafe.AsRef<TPayload>(_payloadPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CommandPayload Slice(int byteOffset)
    {
        return new CommandPayload((byte*)_payloadPtr + byteOffset);
    }
}
