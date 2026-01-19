namespace Axis.ECS;

ref struct ArchetypeColumnSpans(Archetype archetype, Span<Id> entityIds)
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
}

ref struct ArchetypeColumnSpans<T1>(Archetype archetype, Span<Id> entityIds, Span<T1> col1) where T1 : unmanaged
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
    public Span<T1> Col1 = col1;
}

ref struct ArchetypeColumnSpans<T1, T2>(Archetype archetype, Span<Id> entityIds, Span<T1> col1, Span<T2> col2)
    where T1 : unmanaged
    where T2 : unmanaged
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
    public Span<T1> Col1 = col1;
    public Span<T2> Col2 = col2;
}

ref struct ArchetypeColumnSpans<T1, T2, T3>(Archetype archetype, Span<Id> entityIds, Span<T1> col1, Span<T2> col2, Span<T3> col3)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
    public Span<T1> Col1 = col1;
    public Span<T2> Col2 = col2;
    public Span<T3> Col3 = col3;
}

ref struct ArchetypeColumnSpans<T1, T2, T3, T4>(Archetype archetype, Span<Id> entityIds, Span<T1> col1, Span<T2> col2, Span<T3> col3, Span<T4> col4)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
    public Span<T1> Col1 = col1;
    public Span<T2> Col2 = col2;
    public Span<T3> Col3 = col3;
    public Span<T4> Col4 = col4;
}

ref struct ArchetypeColumnSpans<T1, T2, T3, T4, T5>(Archetype archetype, Span<Id> entityIds, Span<T1> col1, Span<T2> col2, Span<T3> col3, Span<T4> col4, Span<T5> col5)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    public Archetype Archetype = archetype;
    public Span<Id> EntityIds = entityIds;
    public Span<T1> Col1 = col1;
    public Span<T2> Col2 = col2;
    public Span<T3> Col3 = col3;
    public Span<T4> Col4 = col4;
    public Span<T5> Col5 = col5;
}



ref struct ArchetypeQueryEnumerator
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans(out var e))
                continue;

            _current = new ArchetypeColumnSpans(a, e);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans Current => _current;
}


ref struct ArchetypeQueryEnumerator<T1> where T1 : unmanaged
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans<T1> _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans<T1>(out var e, out var t1))
                continue;

            _current = new ArchetypeColumnSpans<T1>(a, e, t1);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans<T1> Current => _current;
}

ref struct ArchetypeQueryEnumerator<T1, T2>
    where T1 : unmanaged
    where T2 : unmanaged
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans<T1, T2> _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans<T1, T2>(out var e, out var t1, out var t2))
                continue;

            _current = new ArchetypeColumnSpans<T1, T2>(a, e, t1, t2);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans<T1, T2> Current => _current;
}


ref struct ArchetypeQueryEnumerator<T1, T2, T3>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans<T1, T2, T3> _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans<T1, T2, T3>(out var e, out var t1, out var t2, out var t3))
                continue;

            _current = new ArchetypeColumnSpans<T1, T2, T3>(a, e, t1, t2, t3);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans<T1, T2, T3> Current => _current;
}

ref struct ArchetypeQueryEnumerator<T1, T2, T3, T4>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans<T1, T2, T3, T4> _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans<T1, T2, T3, T4>(out var e, out var t1, out var t2, out var t3, out var t4))
                continue;

            _current = new ArchetypeColumnSpans<T1, T2, T3, T4>(a, e, t1, t2, t3, t4);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans<T1, T2, T3, T4> Current => _current;
}

ref struct ArchetypeQueryEnumerator<T1, T2, T3, T4, T5>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    private readonly Span<Archetype> _archetypes;
    private int _index;
    private ArchetypeColumnSpans<T1, T2, T3, T4, T5> _current;

    public ArchetypeQueryEnumerator(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            ref var a = ref _archetypes[_index];

            if (!a.TryGetColumnSpans<T1, T2, T3, T4, T5>(out var e, out var t1, out var t2, out var t3, out var t4, out var t5))
                continue;

            _current = new ArchetypeColumnSpans<T1, T2, T3, T4, T5>(a, e, t1, t2, t3, t4, t5);
            return true;
        }

        return false;
    }

    public ArchetypeColumnSpans<T1, T2, T3, T4, T5> Current => _current;
}


ref struct ArchetypeQueryEnumerable
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator GetEnumerator()
        => new ArchetypeQueryEnumerator(_archetypes);
}


ref struct ArchetypeQueryEnumerable<T1> where T1 : unmanaged
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator<T1> GetEnumerator()
        => new ArchetypeQueryEnumerator<T1>(_archetypes);
}

ref struct ArchetypeQueryEnumerable<T1, T2>
    where T1 : unmanaged
    where T2 : unmanaged
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator<T1, T2> GetEnumerator()
        => new ArchetypeQueryEnumerator<T1, T2>(_archetypes);
}

ref struct ArchetypeQueryEnumerable<T1, T2, T3>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator<T1, T2, T3> GetEnumerator()
        => new ArchetypeQueryEnumerator<T1, T2, T3>(_archetypes);
}

ref struct ArchetypeQueryEnumerable<T1, T2, T3, T4>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator<T1, T2, T3, T4> GetEnumerator()
        => new ArchetypeQueryEnumerator<T1, T2, T3, T4>(_archetypes);
}

ref struct ArchetypeQueryEnumerable<T1, T2, T3, T4, T5>
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeQueryEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public ArchetypeQueryEnumerator<T1, T2, T3, T4, T5> GetEnumerator()
        => new ArchetypeQueryEnumerator<T1, T2, T3, T4, T5>(_archetypes);
}