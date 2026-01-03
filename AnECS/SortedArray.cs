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
        if (_items.Contains(item))
        {
            return this;
        }

        var newItems = new T[_items.Length + 1];
        Array.Copy(_items, newItems, _items.Length);
        newItems[^1] = item;
        Array.Sort(newItems);

        return new SortedArray<T>(newItems);
    }

    public bool Equals(SortedArray<T> other)
    {
        if (_items.Length != other._items.Length)
        {
            return false;
        }

        for (int i = 0; i < _items.Length; i++)
        {
            if (!_items[i].Equals(other._items[i]))
            {
                return false;
            }
        }

        return true;
    }
}
