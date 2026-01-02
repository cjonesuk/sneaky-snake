using System.Diagnostics;

namespace AnECS;

readonly struct EntityType
{
    private readonly SortedArray<ComponentTypeId> _componentTypeIds;

    public EntityType(Span<ComponentTypeId> componentTypeIds)
    {
        _componentTypeIds = new SortedArray<ComponentTypeId>(componentTypeIds);
    }

    public Span<ComponentTypeId> ComponentTypeIds => _componentTypeIds.AsSpan();

    public bool HasSubset(EntityType other)
    {
        return _componentTypeIds.HasSubset(other._componentTypeIds);
    }

    [Conditional("DEBUG")]
    public void Validate(EntityComponentValue[] components)
    {
        var componentTypeIdsSpan = _componentTypeIds.AsSpan();

        if (components.Length != componentTypeIdsSpan.Length)
        {
            throw new InvalidOperationException($"EntityType expects {componentTypeIdsSpan.Length} components, but {components.Length} were provided");
        }

        foreach (var component in components)
        {
            if (!componentTypeIdsSpan.Contains(component.ComponentTypeId))
            {
                throw new InvalidOperationException($"EntityType does not contain component type ID {component.ComponentTypeId}");
            }
        }
    }
}
