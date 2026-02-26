using System.Collections;

namespace Axis.ECS;

public ref struct ArchetypeEnumerator : IEnumerator<Archetype>
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

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        while (++_index < _archetypes.Length)
        {
            _current = _archetypes[_index];
            return true;
        }

        return false;
    }

    void IDisposable.Dispose()
    {
    }

    void IEnumerator.Reset()
    {
        _index = -1;
        _current = default;
    }
}

