using Axis.Core.Collections;

namespace Axis.ECS.Commands;

using static CommandQueue<World, WorldCommand>;


public readonly struct CommandBase<TContext, TCommand>
{

}

internal readonly struct WorldCommand
{
    public readonly CommandAction Apply;
    public readonly int PayloadOffset;

    public WorldCommand(CommandAction apply, int payloadOffset)
    {
        Apply = apply;
        PayloadOffset = payloadOffset;
    }
}

