namespace Engine.WorldManagement.Entities;

internal readonly struct EntityLocation
{
    public readonly Archetype Archetype;

    /// <summary>
    /// Index of the entity within the archetype's component lists
    /// </summary>
    public readonly int Index;

    internal EntityLocation(Archetype archetype, int index)
    {
        Archetype = archetype;
        Index = index;
    }
}
