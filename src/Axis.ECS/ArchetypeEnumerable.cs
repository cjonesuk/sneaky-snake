namespace Axis.ECS;

public readonly ref struct ArchetypeEnumerable
{
    private readonly Span<Archetype> _archetypes;

    public ArchetypeEnumerable(Span<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public int Count => _archetypes.Length;

    public ArchetypeEnumerator GetEnumerator()
    {
        return new ArchetypeEnumerator(_archetypes);
    }
}

