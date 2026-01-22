using System.Diagnostics.CodeAnalysis;

namespace Axis.ECS;

public sealed class Archetype
{
    private readonly ComponentEntityManager _components;
    private readonly Dictionary<Id, int> _componentIdToColumnIndex;
    private readonly IComponentValues[] _componentColumns;
    private readonly ComponentValues<Id> _entityIds;
    private readonly EntityType _entityType;

    internal Archetype(ComponentEntityManager components, EntityType entityType)
    {
        _components = components;
        _entityType = entityType;

        ReadOnlySpan<Id> componentIds = entityType.ComponentIds;

        _componentIdToColumnIndex = new Dictionary<Id, int>(componentIds.Length);
        _componentColumns = new IComponentValues[componentIds.Length];
        _entityIds = new ComponentValues<Id>();

        for (int index = 0; index < componentIds.Length; index++)
        {
            Id componentId = componentIds[index];
            _componentIdToColumnIndex[componentId] = index;

            _componentColumns[index] = _components.CreateComponentStorage(componentId);
        }
    }

    public EntityType EntityType => _entityType;

    public EntityLocation AddEntity(Id id)
    {
        _components.DebugAssertSupportsEmpty(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        return location;
    }

    public EntityLocation AddEntity<T1>(Id id, ref T1 c1) where T1 : unmanaged
    {
        _components.DebugAssertSupports<T1>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);

        return location;
    }

    public EntityLocation AddEntity<T1, T2>(Id id, ref T1 c1, ref T2 c2) where T1 : unmanaged where T2 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3>(Id id, ref T1 c1, ref T2 c2, ref T3 c3)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3, T4>(Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3, T4>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);
        AppendComponentInternal(ref c4);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3, T4, T5>(Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3, T4, T5>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);
        AppendComponentInternal(ref c4);
        AppendComponentInternal(ref c5);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3, T4, T5, T6>(Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3, T4, T5, T6>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);
        AppendComponentInternal(ref c4);
        AppendComponentInternal(ref c5);
        AppendComponentInternal(ref c6);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3, T4, T5, T6, T7>(Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3, T4, T5, T6, T7>(_entityType);
        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);
        AppendComponentInternal(ref c4);
        AppendComponentInternal(ref c5);
        AppendComponentInternal(ref c6);
        AppendComponentInternal(ref c7);

        return location;
    }

    public EntityLocation AddEntity<T1, T2, T3, T4, T5, T6, T7, T8>(Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7, ref T8 c8)
        where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged where T8 : unmanaged
    {
        _components.DebugAssertSupports<T1, T2, T3, T4, T5, T6, T7, T8>(_entityType);

        EntityLocation location = AppendEntityIdInternal(ref id);

        AppendComponentInternal(ref c1);
        AppendComponentInternal(ref c2);
        AppendComponentInternal(ref c3);
        AppendComponentInternal(ref c4);
        AppendComponentInternal(ref c5);
        AppendComponentInternal(ref c6);
        AppendComponentInternal(ref c7);
        AppendComponentInternal(ref c8);

        return location;
    }

    public bool TrySetComponent<T>(int entityIndex, T component) where T : unmanaged
    {
        if (!TryFindComponentColumn<T>(out var column))
        {
            return false;
        }

        column.Set(entityIndex, component);

        return true;
    }

    private EntityLocation AppendEntityIdInternal(ref Id id)
    {
        int index = _entityIds.Add(ref id);
        return new EntityLocation(this, index);
    }

    private void AppendComponentInternal<T>(ref T component) where T : unmanaged
    {
        var column = FindComponentColumn<T>();
        column.Add(ref component);
    }

    internal bool TryGetColumnSpans(
        out Span<Id> entityIds)
    {
        entityIds = _entityIds.AsSpan();
        return true;
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

    internal bool TryGetColumnSpans<T1, T2, T3>(
        out Span<Id> entityIds,
        out Span<T1> column1,
        out Span<T2> column2,
        out Span<T3> column3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data) ||
            !TryFindComponentColumn<T2>(out var column2Data) ||
            !TryFindComponentColumn<T3>(out var column3Data))
        {
            entityIds = null;
            column1 = null;
            column2 = null;
            column3 = null;

            return false;
        }

        entityIds = _entityIds.AsSpan();
        column1 = column1Data.AsSpan();
        column2 = column2Data.AsSpan();
        column3 = column3Data.AsSpan();

        return true;
    }

    internal bool TryGetColumnSpans<T1, T2, T3, T4>(
        out Span<Id> entityIds,
        out Span<T1> column1,
        out Span<T2> column2,
        out Span<T3> column3,
        out Span<T4> column4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data) ||
            !TryFindComponentColumn<T2>(out var column2Data) ||
            !TryFindComponentColumn<T3>(out var column3Data) ||
            !TryFindComponentColumn<T4>(out var column4Data))
        {
            entityIds = null;
            column1 = null;
            column2 = null;
            column3 = null;
            column4 = null;

            return false;
        }

        entityIds = _entityIds.AsSpan();
        column1 = column1Data.AsSpan();
        column2 = column2Data.AsSpan();
        column3 = column3Data.AsSpan();
        column4 = column4Data.AsSpan();

        return true;
    }

    internal bool TryGetColumnSpans<T1, T2, T3, T4, T5>(
        out Span<Id> entityIds,
        out Span<T1> column1,
        out Span<T2> column2,
        out Span<T3> column3,
        out Span<T4> column4,
        out Span<T5> column5)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data) ||
            !TryFindComponentColumn<T2>(out var column2Data) ||
            !TryFindComponentColumn<T3>(out var column3Data) ||
            !TryFindComponentColumn<T4>(out var column4Data) ||
            !TryFindComponentColumn<T5>(out var column5Data))
        {
            entityIds = null;
            column1 = null;
            column2 = null;
            column3 = null;
            column4 = null;
            column5 = null;

            return false;
        }

        entityIds = _entityIds.AsSpan();
        column1 = column1Data.AsSpan();
        column2 = column2Data.AsSpan();
        column3 = column3Data.AsSpan();
        column4 = column4Data.AsSpan();
        column5 = column5Data.AsSpan();

        return true;
    }

    private ComponentValues<T> FindComponentColumn<T>() where T : unmanaged
    {
        var id = _components.GetId<T>();
        int columnIndex = _componentIdToColumnIndex[id];
        return (ComponentValues<T>)_componentColumns[columnIndex];
    }

    private bool TryFindComponentColumn<T>([NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        var id = _components.GetId<T>();

        if (!_componentIdToColumnIndex.TryGetValue(id, out int columnIndex))
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
        for (int targetComponentIndex = 0; targetComponentIndex < _entityType.ComponentIds.Length; targetComponentIndex++)
        {
            Id targetComponentId = _entityType.ComponentIds[targetComponentIndex];
            int sourceColumnIndex = source.Archetype._componentIdToColumnIndex[targetComponentId];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = _componentIdToColumnIndex[targetComponentId];
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
        for (int sourceComponentIndex = 0; sourceComponentIndex < sourceEntityType.ComponentIds.Length; sourceComponentIndex++)
        {
            Id targetComponentId = sourceEntityType.ComponentIds[sourceComponentIndex];
            int sourceColumnIndex = source.Archetype._componentIdToColumnIndex[targetComponentId];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = _componentIdToColumnIndex[targetComponentId];
            var targetColumn = _componentColumns[targetColumnIndex];

            targetColumn.Migrate(sourceColumn, sourceIndex);
        }

        // Add the new component
        AppendComponentInternal(ref c1);

        return new EntityLocation(this, targetIndex);
    }

    public bool SupportsComponentType<T>() where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        return _componentIdToColumnIndex.ContainsKey(componentId);
    }

    internal ref T GetComponentRef<T>(int index) where T : unmanaged
    {
        var column = FindComponentColumn<T>().AsSpan();
        return ref column[index];
    }

    internal bool TryGetComponentRef<T>(int index, [NotNullWhen(true)] out Ref<T> component) where T : unmanaged
    {
        if (!TryFindComponentColumn<T>(out var column))
        {
            component = default;
            return false;
        }

        var span = column.AsSpan();
        component = new Ref<T>(ref span[index]);
        return true;
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
