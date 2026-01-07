using System.Diagnostics.CodeAnalysis;

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

    public EntityLocation AddEntity(Id id)
    {
        EntityTypeInformation.DebugAssertSupports(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        return location;
    }

    public EntityLocation AddEntity<T1>(Id id, ref T1 c1) where T1 : unmanaged
    {
        EntityTypeInformation<T1>.DebugAssertSupports(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);

        return location;
    }

    public EntityLocation AddEntity<T1, T2>(Id id, ref T1 c1, ref T2 c2) where T1 : unmanaged where T2 : unmanaged
    {
        EntityTypeInformation<T1, T2>.DebugAssertSupports(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);

        return location;
    }

    public void SetComponent<T>(int entityIndex, T component) where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;
        int columnIndex = _componentTypeIdToColumnIndex[componentTypeId];
        var column = (ComponentValues<T>)_componentColumns[columnIndex];
        column.Set(entityIndex, component);
    }

    private EntityLocation AppendEntityIdInternal(ref Id id)
    {
        int index = _entityIds.Add(ref id);
        return new EntityLocation(this, index);
    }

    private void AppendComponentInternal<T>(ref T component) where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;
        int columnIndex = _componentTypeIdToColumnIndex[componentTypeId];
        var column = (ComponentValues<T>)_componentColumns[columnIndex];
        column.Add(ref component);
    }


    internal bool TryGetColumnSpans<T1>(
        out Span<Id> entityIds,
        out Span<T1> column1)
        where T1 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data))
        {
            entityIds = null;
            column1 = null;
            return false;
        }

        entityIds = _entityIds.AsSpan();
        column1 = column1Data.AsSpan();

        return true;
    }

    internal bool TryGetColumnSpans<T1, T2>(
        out Span<Id> entityIds,
        out Span<T1> column1,
        out Span<T2> column2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data) ||
            !TryFindComponentColumn<T2>(out var column2Data))
        {
            entityIds = null;
            column1 = null;
            column2 = null;

            return false;
        }

        entityIds = _entityIds.AsSpan();
        column1 = column1Data.AsSpan();
        column2 = column2Data.AsSpan();

        return true;
    }

    private ComponentValues<T> FindComponentColumn<T>() where T : unmanaged
    {
        var type = ComponentTypeInformation<T>.Id;
        int columnIndex = _componentTypeIdToColumnIndex[type];
        var column = (ComponentValues<T>)_componentColumns[columnIndex];
        return column;
    }

    private bool TryFindComponentColumn<T>([NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        var type = ComponentTypeInformation<T>.Id;
        if (!_componentTypeIdToColumnIndex.TryGetValue(type, out int columnIndex))
        {
            column = null;
            return false;
        }

        column = (ComponentValues<T>)_componentColumns[columnIndex];
        return true;
    }

    /// <summary>
    /// Remove an entities components by migrating it to a simpler archetype.
    /// </summary>
    internal EntityLocation MigrateEntity(EntityLocation source)
    {
        int sourceIndex = source.Index;

        int targetIndex = _entityIds.Count;

        // Migrate the entity ID
        _entityIds.Migrate(source.Archetype._entityIds, sourceIndex);

        // Migrate only components that exist in the target archetype
        for (int targetComponentTypeIndex = 0; targetComponentTypeIndex < _entityType.ComponentTypeIds.Length; targetComponentTypeIndex++)
        {
            ComponentTypeId componentTypeId = _entityType.ComponentTypeIds[targetComponentTypeIndex];
            int sourceColumnIndex = source.Archetype._componentTypeIdToColumnIndex[componentTypeId];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = _componentTypeIdToColumnIndex[componentTypeId];
            var targetColumn = _componentColumns[targetColumnIndex];

            targetColumn.Migrate(sourceColumn, sourceIndex);
        }

        return new EntityLocation(this, targetIndex);
    }

    /// <summary>
    /// Migrate an entity to a more complex archetype, adding in the new component.
    /// </summary> 
    internal EntityLocation MigrateEntity<T>(EntityLocation source, ref T c1) where T : unmanaged
    {
        int sourceIndex = source.Index;

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
            var column = FindComponentColumn<T>();
            column.Add(ref c1);
        }

        return new EntityLocation(this, targetIndex);
    }

    public bool SupportsComponentType<T>() where T : unmanaged
    {
        ComponentTypeId componentTypeId = ComponentTypeInformation<T>.Id;
        return _componentTypeIdToColumnIndex.ContainsKey(componentTypeId);
    }

    internal ref T GetComponentRef<T>(int index) where T : unmanaged
    {
        var column = FindComponentColumn<T>().AsSpan();
        return ref column[index];
    }

    internal void RemoveEntity(int index)
    {
        foreach (var column in _componentColumns)
        {
            column.RemoveAndFillHoleAt(index);
        }
    }

    /// <summary>
    /// Clears all component columns without resizing the underlying arrays.
    /// </summary>
    internal void Clear()
    {
        _entityIds.Clear();

        foreach (var column in _componentColumns)
        {
            column.Clear();
        }
    }
}
