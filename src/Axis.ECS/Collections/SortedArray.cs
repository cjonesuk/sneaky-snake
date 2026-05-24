namespace Axis.ECS.Collections;

readonly struct SortedArray<T> : IEquatable<SortedArray<T>> where T : IComparable<T>
{
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    private readonly T[] _items;

    private SortedArray(T[] items)
    {
        _items = items;
    }

    public static readonly SortedArray<T> Empty = new SortedArray<T>(Array.Empty<T>());

    public static SortedArray<T> Create(ReadOnlySpan<T> items)
    {
        if (items.IsEmpty)
        {
            return Empty;
        }

        var itemsArray = items.ToArray();
        Array.Sort(itemsArray);

        return new SortedArray<T>(itemsArray);
    }

    public ReadOnlySpan<T> AsSpan() => _items;

    public bool HasSubset(SortedArray<T> other)
    {
        var otherArray = other._items;

        int i = 0, j = 0;

        while (i < _items.Length && j < otherArray.Length)
        {
            int cmp = _items[i].CompareTo(otherArray[j]);
            if (cmp == 0)
            {
                j++;
            }
            i++;
        }

        return j == otherArray.Length;
    }

    public bool With(T item, out SortedArray<T> result)
    {
        int index = Array.BinarySearch(_items, item);
        if (index >= 0)
        {
            result = this;
            return false;
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

        result = new SortedArray<T>(newItems);
        return true;
    }

    public bool Without(T item, out SortedArray<T> result)
    {
        int index = Array.BinarySearch(_items, item);
        if (index < 0)
        {
            result = this;
            return false;
        }

        var newItems = new T[_items.Length - 1];
        if (index > 0)
        {
            Array.Copy(_items, 0, newItems, 0, index);
        }
        if (index < _items.Length - 1)
        {
            Array.Copy(_items, index + 1, newItems, index, _items.Length - index - 1);
        }

        result = new SortedArray<T>(newItems);
        return true;
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", _items)}]";
    }

    public override bool Equals(object? obj)
    {
        if (obj is SortedArray<T> other)
        {
            return Equals(other);
        }

        return false;
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

        for (int i = 0; i < _items.Length; i++)
        {
            if (!Comparer.Equals(_items[i], other._items[i]))
            {
                return false;
            }
        }

        return true;
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
