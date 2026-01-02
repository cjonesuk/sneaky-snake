namespace AnECS;

internal sealed class SparseEntity
{
    private readonly Dictionary<ComponentTypeId, object> _components = new();
    private readonly Id _id;

    public SparseEntity(Id id)
    {
        _id = id;
    }

    public Id Id => _id;

    public IReadOnlyDictionary<ComponentTypeId, object> Components => _components;

    public EntityType DetermineType()
    {
        var componentTypeIds = _components.Keys.ToArray();
        return new EntityType(componentTypeIds);
    }

    public void AddComponent(ComponentTypeId componentTypeId, object component)
    {
        _components[componentTypeId] = component;
    }
}
