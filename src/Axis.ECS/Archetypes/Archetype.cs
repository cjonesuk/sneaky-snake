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


    public bool TrySetComponent<T>(int entityIndex, Id componentId, in T component) where T : unmanaged
    {
        if (!TryFindComponentColumnById<T>(componentId, out var column))
        {
            return false;
        }

        column.Set(entityIndex, component);

        return true;
    }

    internal EntityLocation AllocateEntity(in Id id)
    {
        int index = _entityIds.Add(in id);

        foreach (var column in _componentColumns)
        {
            column.AddDefault();
        }

        return new EntityLocation(this, index);
    }

    internal unsafe void WriteComponent(int entityIndex, Id componentId, byte* data, int size)
    {
        var column = FindComponentColumn(componentId);
        column.Write(entityIndex, data, size);
    }

    internal void AppendComponentInternal<T>(Id componentId, in T component) where T : unmanaged
    {
        var column = FindComponentColumn<T>(componentId);
        column.Add(in component);
    }

    public Span<Id> GetEntityIds()
    {
        return _entityIds.AsSpan();
    }

    internal bool TryGetColumnSpan<T1>(
        out Span<T1> column1)
        where T1 : unmanaged
    {
        if (!TryFindComponentColumn<T1>(out var column1Data))
        {
            column1 = null;
            return false;
        }

        column1 = column1Data.AsSpan();

        return true;
    }

    private IComponentValues FindComponentColumn(Id componentId)
    {
        int columnIndex = _componentIdToColumnIndex[componentId];
        return _componentColumns[columnIndex];
    }

    private ComponentValues<T> FindComponentColumn<T>(Id componentId) where T : unmanaged
    {
        int columnIndex = _componentIdToColumnIndex[componentId];
        return (ComponentValues<T>)_componentColumns[columnIndex];
    }

    private bool TryFindComponentColumnById<T>(Id componentId, [NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        if (!_componentIdToColumnIndex.TryGetValue(componentId, out int columnIndex))
        {
            column = null;
            return false;
        }

        column = (ComponentValues<T>)_componentColumns[columnIndex];
        return true;
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
    internal EntityLocation MigrateEntityDown(EntityLocation source)
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
    internal EntityLocation MigrateEntityUp<T>(EntityLocation source, Id componentId, ref T c1) where T : unmanaged
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
        AppendComponentInternal(componentId, in c1);

        return new EntityLocation(this, targetIndex);
    }

    public bool SupportsComponentType<T>() where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        return _componentIdToColumnIndex.ContainsKey(componentId);
    }

    internal ref T GetComponentRef<T>(int index) where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        var column = FindComponentColumn<T>(componentId).AsSpan();
        return ref column[index];
    }

    internal bool TryGetComponentRef<T>(int index, [NotNullWhen(true)] out Ref<T> component) where T : unmanaged
    {
        Id componentId = _components.GetId<T>();

        if (!TryFindComponentColumnById<T>(componentId, out var column))
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
