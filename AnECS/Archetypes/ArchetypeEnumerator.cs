namespace AnECS;

struct ArchetypeEnumerator
{
    private readonly List<Archetype> _list;
    private readonly EntityType _filter;
    private int _index;
    private Archetype? _current;

    public ArchetypeEnumerator(List<Archetype> list, EntityType filter)
    {
        _list = list;
        _filter = filter;
        _index = 0;
        _current = null;
    }

    public Archetype Current => _current!;

    public bool MoveNext()
    {
        while (_index < _list.Count)
        {
            var a = _list[_index++];

            if (Supports(a, _filter))
            {
                _current = a;
                return true;
            }
        }

        return false;
    }

    private static bool Supports(Archetype archetype, EntityType requirement)
    {
        return archetype.EntityType.HasSubset(requirement);
    }
}