using Axis.Core.Memory;

namespace Axis.Core.Collections;

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
