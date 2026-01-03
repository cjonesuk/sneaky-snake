using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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

    public bool With(ComponentTypeId componentTypeId, [MaybeNullWhen(false)] out EntityType extendedType)
    {
        if (_componentTypeIds.With(componentTypeId, out var extendedComponentTypeIds))
        {
            extendedType = new EntityType(extendedComponentTypeIds);
            return true;
        }

        extendedType = default;
        return false;
    }

    public bool Without(ComponentTypeId componentTypeId, [MaybeNullWhen(false)] out EntityType reducedType)
    {
        if (_componentTypeIds.Without(componentTypeId, out var reducedComponentTypeIds))
        {
            reducedType = new EntityType(reducedComponentTypeIds);
            return true;
        }

        reducedType = default;
        return false;
    }

    public bool Equals(EntityType other)
    {
        return _componentTypeIds.Equals(other._componentTypeIds);
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
