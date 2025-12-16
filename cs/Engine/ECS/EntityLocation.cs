namespace Engine;

internal struct EntityLocation
{
    public Archetype Archetype;

    /// <summary>
    /// Index of the entity within the archetype's component lists
    /// </summary>
    public int Index;

    internal EntityLocation(Archetype archetype, int index)
    {
        Archetype = archetype;
        Index = index;
    }
}
