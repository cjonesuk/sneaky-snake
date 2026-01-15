using System.Buffers.Text;
using System.Text;
using Axis.Core.Memory;

namespace Axis.Core.Text;

// todo: tests
public ref struct Utf8StringBuilder
{
    private Span<byte> _buffer;
    private int _pos;

    public Utf8StringBuilder(Span<byte> buffer)
    {
        _buffer = buffer;
        _pos = 0;
    }

    public readonly int Length => _pos;

    public void Write(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
        {
            return;
        }

        _pos += Encoding.UTF8.GetBytes(text, _buffer[_pos..]);
    }

    public void Write(ReadOnlySpan<byte> utf8)
    {
        if (utf8.IsEmpty)
        {
            return;
        }

        utf8.CopyTo(_buffer[_pos..]);
        _pos += utf8.Length;
    }

    public void WriteInt(int value)
    {
        if (!Utf8Formatter.TryFormat(value, _buffer[_pos..], out int written))
        {
            throw new InvalidOperationException("Buffer too small");
        }

        _pos += written;
    }

    public void WriteFloat(float value, string format = "G")
    {
        _pos += Encoding.UTF8.GetBytes(
            value.ToString(format, System.Globalization.CultureInfo.InvariantCulture),
            _buffer[_pos..]);
    }

    public void WriteChar(char c)
    {
        Span<char> tmpChar = [c];
        _pos += Encoding.UTF8.GetBytes(tmpChar, _buffer[_pos..]);
    }

    public ReadOnlySpan<byte> AsSpan() => _buffer[.._pos];

    public NativeSlice CommitTo(NativeBuffer buffer, bool addNull = true)
    {
        return buffer.WriteBytes(AsSpan(), addNull);
    }
}