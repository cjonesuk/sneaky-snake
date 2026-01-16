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