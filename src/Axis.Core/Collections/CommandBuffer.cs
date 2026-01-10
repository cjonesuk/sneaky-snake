using System.Runtime.CompilerServices;
using Axis.Core.Memory;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TPayload GetRef<TPayload>()
        where TPayload : unmanaged
    {
        return ref Unsafe.AsRef<TPayload>(_payloadPtr);
    }
}


public abstract class CommandQueue<TContext, TCommand>
{
    protected readonly List<TCommand> _commands;
    protected readonly NativeBuffer _payload;

    public delegate void CommandAction(ref TContext context, CommandPayload payload);


    public CommandQueue()
    {
        _commands = new List<TCommand>();
        _payload = new NativeBuffer(1024);
    }

    public void Clear()
    {
        _commands.Clear();
        _payload.Reset();
    }
}
