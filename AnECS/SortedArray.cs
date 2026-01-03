namespace AnECS;

readonly struct SortedArray<T> : IEquatable<SortedArray<T>> where T : IComparable<T>
{
    private readonly T[] _items;

    private SortedArray(T[] items)
    {
        _items = items;
    }

    public static readonly SortedArray<T> Empty = new SortedArray<T>(Array.Empty<T>());

    public static SortedArray<T> Create(Span<T> items)
    {
        if (items.IsEmpty)
        {
            return Empty;
        }

        var itemsArray = items.ToArray();
        Array.Sort(itemsArray);

        return new SortedArray<T>(itemsArray);
    }

    public Span<T> AsSpan() => _items;

    public bool HasSubset(SortedArray<T> other)
    {
        IComparer<T> comparer = Comparer<T>.Default;

        var thisSpan = _items.AsSpan();
        var otherSpan = other._items.AsSpan();
        int thisIndex = 0;
        int otherIndex = 0;

        while (thisIndex < thisSpan.Length && otherIndex < otherSpan.Length)
        {
            int comparison = comparer.Compare(thisSpan[thisIndex], otherSpan[otherIndex]);
            if (comparison == 0)
            {
                otherIndex++;
            }
            thisIndex++;
        }

        return otherIndex == otherSpan.Length;
    }

    public SortedArray<T> WithItem(T item)
    {
        int index = Array.BinarySearch(_items, item);
        if (index >= 0)
        {
            return this;
        }

        index = ~index;

        T[] newItems = new T[_items.Length + 1];
        if (index > 0)
        {
            Array.Copy(_items, 0, newItems, 0, index);
        }

        newItems[index] = item;

        if (index < _items.Length)
        {
            Array.Copy(_items, index, newItems, index + 1, _items.Length - index);
        }

        return new SortedArray<T>(newItems);
    }

    public bool Equals(SortedArray<T> other)
    {
        if (ReferenceEquals(_items, other._items))
        {
            return true;
        }

        if (_items.Length != other._items.Length)
        {
            return false;
        }

        return _items.SequenceEqual(other._items);
    }

    public override int GetHashCode()
    {
        var span = _items.AsSpan();

        var hashCode = new HashCode();

        foreach (var i in span)
        {
            hashCode.Add(i);
        }

        return hashCode.ToHashCode();
    }
}
