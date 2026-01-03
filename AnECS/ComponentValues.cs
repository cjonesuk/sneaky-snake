using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AnECS;

internal interface IComponentValues
{
    void Add<TInput>(ref TInput value) where TInput : struct;
    void Migrate(IComponentValues source, int sourceIndex);
}


internal sealed class ComponentValues<T> : IComponentValues where T : struct
{
    private const int DefaultInitialCapacity = 32;

    private T[] _values;
    private int _count;

    public int Count => _count;

    public ComponentValues()
    {
        _values = new T[DefaultInitialCapacity];
    }

    public Span<T> AsSpan()
    {
        return _values.AsSpan(0, _count);
    }

    void IComponentValues.Add<TInput>(ref TInput value) where TInput : struct
    {
        AssertTypeMatch<TInput>();

        if (_count == _values.Length)
        {
            Array.Resize(ref _values, _values.Length * 2);
        }

        _values[_count++] = Unsafe.As<TInput, T>(ref value);
    }

    /// <summary>
    /// Adds the given value to the end of the collection and returns its index.
    /// </summary> 
    public int Add(ref T value)
    {
        if (_count == _values.Length)
        {
            Array.Resize(ref _values, _values.Length * 2);
        }

        int index = _count++;

        _values[index] = value;

        return index;
    }

    public void Set(int index, T value)
    {
        _values[index] = value;
    }

    public void Migrate(IComponentValues source, int sourceIndex)
    {
        var sourceValues = (ComponentValues<T>)source;

        // Append the source value to this collection
        Add(ref sourceValues._values[sourceIndex]);

        // Remove the source value by replacing it with the last value to maintain density
        sourceValues.RemoveAndFillHoleAt(sourceIndex);
    }

    private void RemoveAndFillHoleAt(int index)
    {
        int last = --_count;

        if (index != last)
        {
            _values[index] = _values[last];
        }

        _values[last] = default!;
    }

    internal ref T GetRef(int index)
    {
        return ref _values[index];
    }

    [Conditional("DEBUG")]
    private void AssertTypeMatch<TActual>() where TActual : struct
    {
        if (typeof(TActual) != typeof(T))
        {
            throw new InvalidOperationException($"Invalid type {typeof(TActual)} for ComponentValues of type {typeof(T)}");
        }
    }

}
