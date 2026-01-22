using Axis.Core.Collections;
using Axis.Engine.Resources;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public readonly struct Resolution(int width, int height)
{
    public readonly int Width = width;
    public readonly int Height = height;
}

public readonly struct RenderContext(Resolution resolution, FrameResources frameResources, RenderCommandQueue renderCommands)
{
    public readonly Resolution Resolution = resolution;
    public readonly FrameResources FrameResources = frameResources;
    public readonly RenderCommandQueue RenderCommands = renderCommands;
}

public readonly struct RenderCommand(CommandAction apply, int payloadOffset, int zOrder)
{
    public readonly CommandAction Apply = apply;
    public readonly int PayloadOffset = payloadOffset;
    public readonly int ZOrder = zOrder;
}

public sealed class RenderCommandQueue : CommandQueue<RenderContext, RenderCommand>
{
    public RenderCommandQueue()
    {
    }

    public void Write<T>(ref T value, CommandAction action, int zOrder)
        where T : unmanaged
    {
        int offset = _payload.Alloc(value);
        _commands.Add(new RenderCommand(action, offset, zOrder));
    }

    public unsafe void ApplyAndClear(ref RenderContext context)
    {
        if (_commands.Count == 0)
            return;

        byte* pBase = _payload.Ptr;

        foreach (var command in _commands)
        {
            CommandPayload payload = CommandPayload.From(pBase, command.PayloadOffset);

            command.Apply(ref context, payload);
        }

        Clear();
    }
}