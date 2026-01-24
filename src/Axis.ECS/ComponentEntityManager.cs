using System.Diagnostics;

namespace Axis.ECS;

public record class ComponentEntityMetadata(ComponentTypeId ComponentTypeId, Type ComponentType, Type ComponentValuesType);

public sealed class ComponentEntityManager
{
    private readonly EntityIdManager _entityIds;
    private readonly Dictionary<ComponentTypeId, Id> _componentTypeToEntityId;
    private readonly Dictionary<Id, ComponentEntityMetadata> _componentMetadata;

    internal ComponentEntityManager(EntityIdManager entityIds)
    {
        _entityIds = entityIds;
        _componentTypeToEntityId = new Dictionary<ComponentTypeId, Id>();
        _componentMetadata = new Dictionary<Id, ComponentEntityMetadata>();
    }


    public Id Register<T>() where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;

        if (_componentTypeToEntityId.ContainsKey(componentTypeId))
        {
            throw new InvalidOperationException($"Component type {typeof(T)} is already registered.");
        }

        return RegisterInternal<T>();
    }

    public Id GetId<T>() where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;

        if (_componentTypeToEntityId.TryGetValue(componentTypeId, out Id entityId))
        {
            return entityId;
        }

        return RegisterInternal<T>();
    }

    public ComponentEntityMetadata GetMetadata(Id componentId)
    {
        return _componentMetadata[componentId];
    }

    public IComponentValues CreateComponentStorage(Id componentId)
    {
        var metadata = _componentMetadata[componentId];
        Type componentValuesType = metadata.ComponentValuesType;

        return (IComponentValues)(Activator.CreateInstance(componentValuesType) ?? throw new InvalidOperationException($"Could not create list of type {componentValuesType}"));
    }

    private Id RegisterInternal<T>() where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;

        Id id = _entityIds.AllocateComponentId();
        _componentTypeToEntityId[componentTypeId] = id;

        Type componentType = typeof(T);
        Type componentValuesType = typeof(ComponentValues<T>);
        _componentMetadata[id] = new ComponentEntityMetadata(componentTypeId, componentType, componentValuesType);

        return id;
    }
}

internal static class ComponentEntityManagerExtensions
{
    [Conditional("DEBUG")]
    public static void DebugAssertSupportsEmpty(this ComponentEntityManager manager, EntityType entityType)
    {
        if (!entityType.Equals(EntityType.Empty))
        {
            throw new InvalidOperationException("EntityType (Empty) is not supported.");
        }
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");

        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3, T4>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
        Id componentId4 = manager.GetId<T4>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId4), $"EntityType does not support component type {typeof(T4)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3, T4, T5>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
        Id componentId4 = manager.GetId<T4>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId4), $"EntityType does not support component type {typeof(T4)}");
        Id componentId5 = manager.GetId<T5>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId5), $"EntityType does not support component type {typeof(T5)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3, T4, T5, T6>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
        Id componentId4 = manager.GetId<T4>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId4), $"EntityType does not support component type {typeof(T4)}");
        Id componentId5 = manager.GetId<T5>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId5), $"EntityType does not support component type {typeof(T5)}");
        Id componentId6 = manager.GetId<T6>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId6), $"EntityType does not support component type {typeof(T6)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3, T4, T5, T6, T7>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
        Id componentId4 = manager.GetId<T4>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId4), $"EntityType does not support component type {typeof(T4)}");
        Id componentId5 = manager.GetId<T5>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId5), $"EntityType does not support component type {typeof(T5)}");
        Id componentId6 = manager.GetId<T6>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId6), $"EntityType does not support component type {typeof(T6)}");
        Id componentId7 = manager.GetId<T7>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId7), $"EntityType does not support component type {typeof(T7)}");
    }

    [Conditional("DEBUG")]
    public static void DebugAssertSupports<T1, T2, T3, T4, T5, T6, T7, T8>(this ComponentEntityManager manager, EntityType entityType) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged where T8 : unmanaged
    {
        Id componentId1 = manager.GetId<T1>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId1), $"EntityType does not support component type {typeof(T1)}");
        Id componentId2 = manager.GetId<T2>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId2), $"EntityType does not support component type {typeof(T2)}");
        Id componentId3 = manager.GetId<T3>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId3), $"EntityType does not support component type {typeof(T3)}");
        Id componentId4 = manager.GetId<T4>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId4), $"EntityType does not support component type {typeof(T4)}");
        Id componentId5 = manager.GetId<T5>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId5), $"EntityType does not support component type {typeof(T5)}");
        Id componentId6 = manager.GetId<T6>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId6), $"EntityType does not support component type {typeof(T6)}");
        Id componentId7 = manager.GetId<T7>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId7), $"EntityType does not support component type {typeof(T7)}");
        Id componentId8 = manager.GetId<T8>();
        Debug.Assert(entityType.ComponentIds.Contains(componentId8), $"EntityType does not support component type {typeof(T8)}");
    }

    public static EntityType GetEntityType<T1>(this ComponentEntityManager components) where T1 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        return EntityType.Create([componentId1]);
    }

    public static EntityType GetEntityType<T1, T2>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        return EntityType.Create([componentId1, componentId2]);
    }

    public static EntityType GetEntityType<T1, T2, T3>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        return EntityType.Create([componentId1, componentId2, componentId3]);
    }

    public static EntityType GetEntityType<T1, T2, T3, T4>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        Id componentId4 = components.GetId<T4>();
        return EntityType.Create([componentId1, componentId2, componentId3, componentId4]);
    }

    public static EntityType GetEntityType<T1, T2, T3, T4, T5>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        Id componentId4 = components.GetId<T4>();
        Id componentId5 = components.GetId<T5>();
        return EntityType.Create([componentId1, componentId2, componentId3, componentId4, componentId5]);
    }

    public static EntityType GetEntityType<T1, T2, T3, T4, T5, T6>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        Id componentId4 = components.GetId<T4>();
        Id componentId5 = components.GetId<T5>();
        Id componentId6 = components.GetId<T6>();
        return EntityType.Create([componentId1, componentId2, componentId3, componentId4, componentId5, componentId6]);
    }

    public static EntityType GetEntityType<T1, T2, T3, T4, T5, T6, T7>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        Id componentId4 = components.GetId<T4>();
        Id componentId5 = components.GetId<T5>();
        Id componentId6 = components.GetId<T6>();
        Id componentId7 = components.GetId<T7>();
        return EntityType.Create([componentId1, componentId2, componentId3, componentId4, componentId5, componentId6, componentId7]);
    }

    public static EntityType GetEntityType<T1, T2, T3, T4, T5, T6, T7, T8>(this ComponentEntityManager components)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged
    {
        Id componentId1 = components.GetId<T1>();
        Id componentId2 = components.GetId<T2>();
        Id componentId3 = components.GetId<T3>();
        Id componentId4 = components.GetId<T4>();
        Id componentId5 = components.GetId<T5>();
        Id componentId6 = components.GetId<T6>();
        Id componentId7 = components.GetId<T7>();
        Id componentId8 = components.GetId<T8>();
        return EntityType.Create([componentId1, componentId2, componentId3, componentId4, componentId5, componentId6, componentId7, componentId8]);
    }
}
