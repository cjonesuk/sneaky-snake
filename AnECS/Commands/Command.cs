namespace AnECS.Commands;

internal readonly struct Command
{
    public readonly CommandAction Apply;
    public readonly int PayloadOffset;

    public Command(CommandAction apply, int payloadOffset)
    {
        Apply = apply;
        PayloadOffset = payloadOffset;
    }
}

