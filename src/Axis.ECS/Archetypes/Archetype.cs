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
        int columnIndex = FindColumnIndex(componentId);
        IComponentValues column = _componentColumns[columnIndex];
        T local = component;
        column.Add<T>(ref local);
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

    private bool TryFindComponentColumnById<T>(Id componentId, [NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        int columnIndex = FindColumnIndex(componentId);
        if (columnIndex < 0)
        {
            column = null;
            return false;
        }

        IComponentValues raw = _componentColumns[columnIndex];
        if (raw is TagComponentValues)
        {
            throw new InvalidOperationException($"'{typeof(T).Name}' is a tag; values cannot be read.");
        }

        column = (ComponentValues<T>)raw;
        return true;
    }

    private bool TryFindComponentColumn<T>([NotNullWhen(true)] out ComponentValues<T>? column) where T : unmanaged
    {
        return TryFindComponentColumnById<T>(_components.GetId<T>(), out column);
    }

    /// <summary>
    /// Remove an entity's components by migrating it to a simpler archetype.
    /// Returns where the migrated entity now lives, and (separately) the entity that was
    /// displaced from the source archetype's last slot to fill the gap (<see cref="EntityRef.None"/> if none).
    /// </summary>
    internal (EntityLocation MigratedTo, EntityRef Displaced) MigrateEntityDown(EntityLocation source)
    {
        int sourceIndex = source.Index;
        int targetIndex = _entityIds.Count;
        EntityRef displaced = source.Archetype.PeekMoverOnRemoval(sourceIndex);

        _entityIds.AddFrom(source.Archetype._entityIds, sourceIndex);
        source.Archetype._entityIds.RemoveAndFillHoleAt(sourceIndex);

        ReadOnlySpan<Id> sourceIds = source.Archetype._entityType.ComponentIds;
        for (int sourceColumnIndex = 0; sourceColumnIndex < sourceIds.Length; sourceColumnIndex++)
        {
            Id sourceComponentId = sourceIds[sourceColumnIndex];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = FindColumnIndex(sourceComponentId);
            if (targetColumnIndex >= 0)
            {
                _componentColumns[targetColumnIndex].AddFrom(sourceColumn, sourceIndex);
            }

            sourceColumn.RemoveAndFillHoleAt(sourceIndex);
        }

        return (new EntityLocation(this, targetIndex), displaced);
    }

    /// <summary>
    /// Migrate an entity to a more complex archetype, adding in the new component.
    /// Returns where the migrated entity now lives, and (separately) the entity that was
    /// displaced from the source archetype's last slot to fill the gap (<see cref="EntityRef.None"/> if none).
    /// </summary>
    internal (EntityLocation MigratedTo, EntityRef Displaced) MigrateEntityUp<T>(EntityLocation source, Id componentId, ref T c1) where T : unmanaged
    {
        int sourceIndex = source.Index;
        int targetIndex = _entityIds.Count;
        EntityRef displaced = source.Archetype.PeekMoverOnRemoval(sourceIndex);

        _entityIds.AddFrom(source.Archetype._entityIds, sourceIndex);
        source.Archetype._entityIds.RemoveAndFillHoleAt(sourceIndex);

        ReadOnlySpan<Id> sourceIds = source.Archetype._entityType.ComponentIds;
        for (int sourceColumnIndex = 0; sourceColumnIndex < sourceIds.Length; sourceColumnIndex++)
        {
            Id sharedComponentId = sourceIds[sourceColumnIndex];
            var sourceColumn = source.Archetype._componentColumns[sourceColumnIndex];

            int targetColumnIndex = FindColumnIndex(sharedComponentId);
            _componentColumns[targetColumnIndex].AddFrom(sourceColumn, sourceIndex);
            sourceColumn.RemoveAndFillHoleAt(sourceIndex);
        }

        AppendComponentInternal(componentId, in c1);

        return (new EntityLocation(this, targetIndex), displaced);
    }

    public bool SupportsComponentType<T>() where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        return FindColumnIndex(componentId) >= 0;
    }

    internal ref T GetComponentRef<T>(int index) where T : unmanaged
    {
        Id componentId = _components.GetId<T>();
        if (!TryFindComponentColumnById<T>(componentId, out var column))
        {
            throw new InvalidOperationException($"Component '{typeof(T).Name}' not present on archetype.");
        }
        return ref column.AsSpan()[index];
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

    /// <summary>Remove the entity at <paramref name="index"/>; returns the displaced entity (<see cref="EntityRef.None"/> if none).</summary>
    internal EntityRef RemoveEntity(int index)
    {
        EntityRef displaced = PeekMoverOnRemoval(index);

        foreach (var column in _componentColumns)
        {
            column.RemoveAndFillHoleAt(index);
        }
        _entityIds.RemoveAndFillHoleAt(index);

        return displaced;
    }

    /// <summary>The entity that would be displaced into <paramref name="removalIndex"/> on a swap-pop there; <see cref="EntityRef.None"/> if removing the last entry (no swap needed).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EntityRef PeekMoverOnRemoval(int removalIndex)
    {
        int lastIndex = _entityIds.Count - 1;
        if (removalIndex >= lastIndex) return EntityRef.None;
        Id movedId = _entityIds.AsSpan()[lastIndex];
        return new EntityRef(movedId, this, removalIndex);
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
