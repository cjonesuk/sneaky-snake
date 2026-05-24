namespace Axis.ECS;

public readonly ref struct ArchetypeEnumerable
{
    private readonly ReadOnlySpan<Archetype> _archetypes;

    public ArchetypeEnumerable(ReadOnlySpan<Archetype> archetypes)
    {
        _archetypes = archetypes;
    }

    public int Count => _archetypes.Length;

    public ArchetypeEnumerator GetEnumerator()
    {
        return new ArchetypeEnumerator(_archetypes);
    }
}

