using System.Runtime.InteropServices;

namespace AnECS;

internal sealed class ComponentValues<T> : IComponentValues
{
    private readonly List<T> _values = new();

    public int Count => _values.Count;

    public Span<T> AsSpan()
    {
        return CollectionsMarshal.AsSpan(_values);
    }

    public void Add(object value)
    {
        _values.Add((T)value);
    }

    public void Migrate(IComponentValues source, int sourceIndex)
    {
        var sourceValues = (ComponentValues<T>)source;
        _values.Add(sourceValues._values[sourceIndex]);

        // Remove the source value by replacing it with the last value to maintain density
        sourceValues.RemoveAt(sourceIndex);
    }

    private void RemoveAt(int index)
    {
        int lastIndex = _values.Count - 1;
        _values[index] = _values[lastIndex];
        _values.RemoveAt(lastIndex);
    }
}
