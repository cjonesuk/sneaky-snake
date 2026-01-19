using Axis.Core.Memory;

namespace Axis.Engine.Resources;

public sealed class FrameResources
{
    public const int InitialTextBufferSize = 16 * 1024; // 16 KB

    private readonly NativeBuffer _textBuffer;

    public FrameResources()
    {
        _textBuffer = new NativeBuffer(InitialTextBufferSize);
    }

    public NativeBuffer TextBuffer => _textBuffer;

    public void Reset()
    {
        _textBuffer.Reset();
    }
}