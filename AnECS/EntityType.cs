using System.Diagnostics;

namespace AnECS;

public readonly struct EntityType : IEquatable<EntityType>
{
    public readonly static EntityType Empty = new EntityType(SortedArray<ComponentTypeId>.Empty);

    private readonly SortedArray<ComponentTypeId> _componentTypeIds;

    private EntityType(SortedArray<ComponentTypeId> componentTypeIds)
    {
        _componentTypeIds = componentTypeIds;
    }

    public static EntityType Create(Span<ComponentTypeId> componentTypeIds)
    {
        var sortedArray = SortedArray<ComponentTypeId>.Create(componentTypeIds);
        return new EntityType(sortedArray);
    }

    public Span<ComponentTypeId> ComponentTypeIds => _componentTypeIds.AsSpan();

    public bool HasSubset(EntityType other)
    {
        return _componentTypeIds.HasSubset(other._componentTypeIds);
    }

    public EntityType WithAddedComponent(ComponentTypeId componentTypeId)
    {
        var newComponentTypeIds = _componentTypeIds.WithItem(componentTypeId);
        return new EntityType(newComponentTypeIds);
    }

    public bool Equals(EntityType other)
    {
        return ReferenceEquals(_componentTypeIds, other._componentTypeIds) || _componentTypeIds.Equals(other._componentTypeIds);
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
