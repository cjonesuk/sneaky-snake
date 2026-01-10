using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal sealed class WorldCommandQueue : CommandQueue<World, WorldCommand>
{
    public WorldCommandQueue()
    {
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

        foreach (var cmd in _commands)
        {
            void* payloadPtr = pBase + cmd.PayloadOffset;
            CommandPayload payload = new CommandPayload(payloadPtr);

            cmd.Apply(ref world, payload);
        }

        Clear();
    }
}
