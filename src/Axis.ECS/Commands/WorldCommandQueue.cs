using Axis.Core.Collections;
using Axis.Core.Memory;

namespace Axis.ECS.Commands;

internal sealed class WorldCommandQueue : CommandQueue<World, WorldCommand>
{
    public WorldCommandQueue()
    {
    }

    public NativeBuffer Payload => _payload;

    public int BeginPayload()
    {
        return _payload.Used;
    }

    public void Enqueue(
        CommandAction action,
        int payloadOffset)
    {
        _commands.Add(new WorldCommand(action, payloadOffset));
    }

    public void Write<T>(ref T value, CommandAction action)
        where T : unmanaged
    {
        int offset = _payload.Alloc(value);
        _commands.Add(new WorldCommand(action, offset));
    }

    public unsafe void ApplyAndClear(World world)
    {
        if (_commands.Count == 0)
            return;

        byte* pBase = _payload.Ptr;

        foreach (var command in _commands)
        {
            CommandPayload payload = CommandPayload.From(pBase, command.PayloadOffset);

            command.Apply(ref world, payload);
        }

        Clear();
    }
}
