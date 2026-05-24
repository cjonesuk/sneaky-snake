namespace Axis.Core.Collections;

public ref struct FastBuffer<T>
{
    private Span<T> _buffer;
    private T[]? _heapBuffer;
    private int _count;

    private FastBuffer(Span<T> buffer)
    {
        _buffer = buffer;
        _heapBuffer = null;
        _count = 0;
    }

    public static FastBuffer<T> Create(int capacity)
    {
        T[] heapBuffer = new T[capacity];
        return new FastBuffer<T>(heapBuffer);
    }

    public static FastBuffer<T> Create(Span<T> buffer)
    {
        return new FastBuffer<T>(buffer);
    }

    public ReadOnlySpan<T> AsSpan() => _buffer[.._count];
    public int Count => _count;

    public T[] ToArray()
    {
        T[] array = new T[_count];
        AsSpan().CopyTo(array);
        return array;
    }

    public void Add(in T item)
    {
        if (_count >= _buffer.Length)
        {
            Grow();
        }

        _buffer[_count++] = item;
    }

    private void Grow()
    {
        int newSize = Math.Max(_buffer.Length * 2, 4);

        T[] newArray = new T[newSize];

        _buffer.CopyTo(newArray);

        _buffer = newArray;
        _heapBuffer = newArray;
    }
}

