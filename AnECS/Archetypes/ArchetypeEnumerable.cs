namespace AnECS;

readonly struct ArchetypeEnumerable
{
    private readonly List<Archetype> _list;
    private readonly EntityType _filter;

    public ArchetypeEnumerable(List<Archetype> list, EntityType filter)
    {
        _list = list;
        _filter = filter;
    }

    public ArchetypeEnumerator GetEnumerator()
    {
        return new ArchetypeEnumerator(_list, _filter);
    }
}
