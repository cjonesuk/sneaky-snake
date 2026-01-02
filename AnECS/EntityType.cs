namespace AnECS;

readonly struct EntityType
{
    private readonly SortedArray<int> _componentTypeIds;

    public EntityType(Span<int> componentTypeIds)
    {
        _componentTypeIds = new SortedArray<int>(componentTypeIds);
    }

    public bool HasSubset(EntityType other)
    {
        return _componentTypeIds.HasSubset(other._componentTypeIds);
    }
}
