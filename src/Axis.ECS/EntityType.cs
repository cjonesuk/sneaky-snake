using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Axis.ECS.Collections;

namespace Axis.ECS;

public readonly struct EntityType : IEquatable<EntityType>
{
    public readonly static EntityType Empty = new EntityType(SortedArray<Id>.Empty);

    private readonly SortedArray<Id> _componentIds;

    private EntityType(SortedArray<Id> componentIds)
    {
        _componentIds = componentIds;
    }

    public static EntityType Create(ReadOnlySpan<Id> componentIds)
    {
        var sortedArray = SortedArray<Id>.Create(componentIds);
        return new EntityType(sortedArray);
    }

    public Span<Id> ComponentIds => _componentIds.AsSpan();

    public bool HasSubset(EntityType other)
    {
        return _componentIds.HasSubset(other._componentIds);
    }

    public bool With(Id componentId, [MaybeNullWhen(false)] out EntityType extendedType)
    {
        if (_componentIds.With(componentId, out var extendedComponentIds))
        {
            extendedType = new EntityType(extendedComponentIds);
            return true;
        }

        extendedType = default;
        return false;
    }

    public bool Without(Id componentId, [MaybeNullWhen(false)] out EntityType reducedType)
    {
        if (_componentIds.Without(componentId, out var reducedComponentIds))
        {
            reducedType = new EntityType(reducedComponentIds);
            return true;
        }

        reducedType = default;
        return false;
    }

    public bool Equals(EntityType other)
    {
        return _componentIds.Equals(other._componentIds);
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
        return _componentIds.GetHashCode();
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
        return $"EntityType[{_componentIds}]";
    }
}
