using System.Diagnostics;
using System.Runtime.CompilerServices;
using Axis.Core.Memory;

namespace Axis.ECS;

/// <summary>
/// Operations for managing a single column of component data for an archetype.
/// These operations should not be used during iteration.
/// </summary>
internal interface IComponentValues
{
    void AddDefault();
    void Add<TInput>(ref TInput value) where TInput : unmanaged;
    void RemoveAndFillHoleAt(int index);
    void AddFrom(IComponentValues source, int sourceIndex);
    void Clear();
    unsafe void Write(int entityIndex, byte* data, int size);
}

internal sealed class ComponentValues<T> : IComponentValues where T : unmanaged
{
    private const int DefaultInitialCapacity = 32;

    private T[] _values;
    private int _count;

    public int Count => _count;

    public ComponentValues()
    {
        _values = new T[DefaultInitialCapacity];
    }

    internal Span<T> AsSpan()
    {
        return _values.AsSpan(0, _count);
    }

    /// <summary>
    /// Adds the given value to the end of the collection and returns its index.
    /// </summary> 
    internal int Add(in T value)
    {
        if (_count == _values.Length)
        {
            Array.Resize(ref _values, _values.Length * 2);
        }

        int index = _count++;

        _values[index] = value;

        return index;
    }



    internal void RemoveAndFillHoleAt(int index)
    {
        int last = --_count;

        if (index != last)
        {
            _values[index] = _values[last];
        }

        _values[last] = default!;
    }

    unsafe void IComponentValues.Write(int entityIndex, byte* data, int size)
    {
#if DEBUG
        if (size != NativeType<T>.Size)
        {
            throw new InvalidOperationException("Component size mismatch.");
        }
#endif

        ref T destination = ref _values[entityIndex];

        Unsafe.CopyBlockUnaligned(
            Unsafe.AsPointer(ref destination),
            data,
            (uint)size);
    }

    internal void Set(int index, T value)
    {
        if (index >= _count || index < 0)
        {
            throw new IndexOutOfRangeException($"Index {index} is out of range for ComponentValues with count {_count}");
        }

        _values[index] = value;
    }

    void IComponentValues.AddDefault()
    {
        Add(default!);
    }

    void IComponentValues.Add<TInput>(ref TInput value)
    {
        AssertTypeMatch<TInput>();

        ref T castedRef = ref Unsafe.As<TInput, T>(ref value);

        Add(in castedRef);
    }

    void IComponentValues.Clear() => Clear();

    void IComponentValues.RemoveAndFillHoleAt(int index)
    {
        RemoveAndFillHoleAt(index);
    }

    void IComponentValues.AddFrom(IComponentValues source, int sourceIndex)
    {
        var sourceValues = (ComponentValues<T>)source;
        AddFrom(sourceValues, sourceIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddFrom(ComponentValues<T> sourceValues, int sourceIndex)
    {
        Add(in sourceValues._values[sourceIndex]);
    }

    internal void Clear()
    {
        for (int index = 0; index < _count; index++)
        {
            _values[index] = default!;
        }

        _count = 0;
    }

    [Conditional("DEBUG")]
    private void AssertTypeMatch<TActual>() where TActual : unmanaged
    {
        if (typeof(TActual) != typeof(T))
        {
            throw new InvalidOperationException($"Invalid type {typeof(TActual)} for ComponentValues of type {typeof(T)}");
        }
    }
}
