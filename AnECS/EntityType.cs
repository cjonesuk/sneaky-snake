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
    public void DebugAssertSupports(EntityType actualType)
    {
        if (!actualType.Equals(this))
        {
            throw new InvalidOperationException($"EntityType {actualType} is not compatbile with expected type {this}");
        }
    }

    public override int GetHashCode()
    {
        return _componentTypeIds.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is EntityType other)
        {
            return Equals(other);
        }

        return false;
    }

    public override string ToString()
    {
        return $"EntityType[{_componentTypeIds}]";
    }
}
