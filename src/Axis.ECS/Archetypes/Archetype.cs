using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Axis.ECS;

public sealed class Archetype
{
    private readonly World _world;
    private readonly ComponentEntityManager _components;
    private readonly IComponentValues[] _componentColumns;
    private readonly ComponentValues<Id> _entityIds;
    private readonly EntityType _entityType;

    internal Archetype(World world, ComponentEntityManager components, EntityType entityType)
    {
        _world = world;
        _components = components;
        _entityType = entityType;

        ReadOnlySpan<Id> componentIds = entityType.ComponentIds;

        _componentColumns = new IComponentValues[componentIds.Length];
        _entityIds = new ComponentValues<Id>();

        for (int index = 0; index < componentIds.Length; index++)
        {
            _componentColumns[index] = _components.CreateComponentStorage(componentIds[index]);
        }
    }

    public World World => _world;
    public EntityType EntityType => _entityType;
    public int EntityCount => _entityIds.Count;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindColumnIndex(Id componentId)
    {
        return _entityType.ComponentIds.IndexOf(componentId);
    }

    private IComponentValues FindComponentColumn(Id componentId)
    {
        int columnIndex = FindColumnIndex(componentId);
        return _componentColumns[columnIndex];
    }

    private ComponentValues<T> FindComponentColumn<T>(Id componentId) where T : unmanaged
    {
        int columnIndex = FindColumnIndex(componentId);
        return (ComponentValues<T>)_componentColumns[columnIndex];
    }

    private bool TryFindComponentColumnById<T>(Id componentId, [NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        int columnIndex = FindColumnIndex(componentId);
        if (columnIndex < 0)
        {
            column = null;
            return false;
        }

        column = (ComponentValues<T>)_componentColumns[columnIndex];
        return true;
    }

    private bool TryFindComponentColumn<T>([NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        return TryFindComponentColumnById<T>(_components.GetId<T>(), out column);
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
        ReadOnlySpan<Id> targetIds = _entityType.ComponentIds;
        for (int targetColumnIndex = 0; targetColumnIndex < targetIds.Length; targetColumnIndex++)
        {
            Id targetComponentId = targetIds[targetColumnIndex];
            int sourceColumnIndex = source.Archetype.FindColumnIndex(targetComponentId);
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

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
        ReadOnlySpan<Id> sourceIds = source.Archetype._entityType.ComponentIds;
        for (int sourceColumnIndex = 0; sourceColumnIndex < sourceIds.Length; sourceColumnIndex++)
        {
            Id sharedComponentId = sourceIds[sourceColumnIndex];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = FindColumnIndex(sharedComponentId);
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
        return FindColumnIndex(componentId) >= 0;
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
