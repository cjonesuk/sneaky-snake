namespace Engine;

internal class ArchetypeRow
{
    public readonly EntityId Id;
    public readonly Dictionary<Type, object> Components;

    public ArchetypeRow(EntityId id, Dictionary<Type, object> components)
    {
        Id = id;
        Components = components;
    }
}
