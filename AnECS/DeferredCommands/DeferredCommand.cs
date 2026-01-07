namespace AnECS;

readonly struct DeferredCommand
{
    public readonly DeferredCommandAction Apply;
    public readonly int PayloadOffset;

    public DeferredCommand(DeferredCommandAction apply, int payloadOffset)
    {
        Apply = apply;
        PayloadOffset = payloadOffset;
    }
}

