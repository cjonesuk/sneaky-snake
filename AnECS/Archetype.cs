namespace AnECS;

internal sealed class Archetype
{
    private readonly Dictionary<ComponentTypeId, int> _componentTypeIdToColumnIndex;
    private readonly IComponentValues[] _componentColumns;
    private readonly ComponentValues<Id> _entityIds;
    private readonly EntityType _entityType;


    internal Archetype(EntityType entityType)
    {
        Span<ComponentTypeId> componentTypeIds = entityType.ComponentTypeIds;

        _componentTypeIdToColumnIndex = new Dictionary<ComponentTypeId, int>(componentTypeIds.Length);
        _componentColumns = new IComponentValues[componentTypeIds.Length];
        _entityIds = new ComponentValues<Id>();

        _entityType = entityType;

        for (int index = 0; index < componentTypeIds.Length; index++)
        {
            ComponentTypeId typeId = componentTypeIds[index];
            _componentTypeIdToColumnIndex[typeId] = index;

            Type componentType = ComponentTypeRegistry.GetTypeById(typeId);
            Type componentValuesType = typeof(ComponentValues<>).MakeGenericType(componentType);

            _componentColumns[index] = (IComponentValues)(Activator.CreateInstance(componentValuesType) ?? throw new InvalidOperationException($"Could not create list of type {componentValuesType}"));
        }
    }

    public EntityType EntityType => _entityType;

    public EntityLocation AddEntity(Id id, Span<EntityComponentValue> components)
    {
        if (components.Length != _componentColumns.Length)
        {
            throw new InvalidOperationException($"Archetype expects {_componentColumns.Length} components, but {components.Length} were provided");
        }

        int entityIndex = _entityIds.Count;
        _entityIds.Add(id);

        foreach (var component in components)
        {
            int columnIndex = _componentTypeIdToColumnIndex[component.ComponentTypeId];
            _componentColumns[columnIndex].Add(component.Value);
        }

        return new EntityLocation(this, entityIndex);
    }

    public void SetComponent<T>(int entityIndex, T component) where T : notnull
    {


    }

    internal void Query<T1>(EntityQueryAction<T1> action) where T1 : notnull
    {
        var t1Type = ComponentTypeRegistry.GetComponentTypeId<T1>();
        int t1ColumnIndex = _componentTypeIdToColumnIndex[t1Type];

        var entityIdsSpan = _entityIds.AsSpan();

        var t1Column = (ComponentValues<T1>)_componentColumns[t1ColumnIndex];
        var t1Span = t1Column.AsSpan();

        for (int index = 0; index < _entityIds.Count; index++)
        {
            action(ref entityIdsSpan[index], ref t1Span[index]);
        }
    }

    internal EntityLocation MigrateEntity(EntityLocation source, EntityComponentValue entityComponentValue)
    {
        int sourceIndex = source.ComponentIndex;

        int targetIndex = _entityIds.Count;

        // Migrate the entity ID
        _entityIds.Migrate(source.Archetype._entityIds, sourceIndex);

        // Migrate existing components
        EntityType sourceEntityType = source.Archetype.EntityType;
        for (int sourceComponentTypeIndex = 0; sourceComponentTypeIndex < sourceEntityType.ComponentTypeIds.Length; sourceComponentTypeIndex++)
        {
            ComponentTypeId componentTypeId = sourceEntityType.ComponentTypeIds[sourceComponentTypeIndex];
            int sourceColumnIndex = source.Archetype._componentTypeIdToColumnIndex[componentTypeId];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = _componentTypeIdToColumnIndex[componentTypeId];
            var targetColumn = _componentColumns[targetColumnIndex];

            targetColumn.Migrate(sourceColumn, sourceIndex);
        }

        // Add the new component
        {
            int targetColumnIndex = _componentTypeIdToColumnIndex[entityComponentValue.ComponentTypeId];
            var targetColumn = _componentColumns[targetColumnIndex];
            targetColumn.Add(entityComponentValue.Value);
        }

        return new EntityLocation(this, targetIndex);
    }
}
