namespace Axis.ECS;

public ref struct ArchetypeEnumerator
{
    private ReadOnlySpan<Archetype> _archetypes;
    private int _index;
    private Archetype? _current;

    public ArchetypeEnumerator(ReadOnlySpan<Archetype> archetypes)
    {
        _archetypes = archetypes;
        _index = -1;
        _current = default;
    }

    public Archetype Current => _current!;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            _current = _archetypes[_index];
            return true;
        }

        return false;
    }
}

