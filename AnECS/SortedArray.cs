namespace AnECS;

readonly struct SortedArray<T> where T : IComparable<T>
{
    private readonly T[] _items;

    public SortedArray(Span<T> items)
    {
        _items = items.ToArray();
        Array.Sort(_items);
    }

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
}
