using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace AnECS;




public record struct EntityComponentValue(ComponentTypeId ComponentTypeId, object Value);

record struct EntityLocation(Archetype Archetype, int ComponentIndex);

internal interface IComponentValues
{
    void Add(object value);
    void Migrate(IComponentValues source, int sourceIndex);
}

internal sealed class ComponentValues<T> : IComponentValues
{
    private readonly List<T> _values = new();

    public int Count => _values.Count;

    public Span<T> AsSpan()
    {
        return CollectionsMarshal.AsSpan(_values);
    }

    public void Add(object value)
    {
        _values.Add((T)value);
    }

    public void Migrate(IComponentValues source, int sourceIndex)
    {
        var sourceValues = (ComponentValues<T>)source;
        _values.Add(sourceValues._values[sourceIndex]);
    }
}

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

public interface IWorld
{
    void AddComponentToEntity<T>(Id id, T component) where T : notnull;
}

public delegate void EntityQueryAction<T1>(ref Id id, ref T1 arg1) where T1 : notnull;

internal sealed class World : IWorld
{
    private uint _nextId = 1;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly Dictionary<EntityType, Archetype> _archetypesByEntityType;
    private readonly List<Archetype> _archetypes;
    private readonly Archetype _emptyArchetype;

    internal World()
    {
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _emptyArchetype = new Archetype(EntityType.Empty);
        _archetypesByEntityType = new Dictionary<EntityType, Archetype>()
        {
            { EntityType.Empty, _emptyArchetype }
        };
        _archetypes = new List<Archetype>() { };
    }

    public static World Create()
    {
        return new World();
    }

    public Entity Entity()
    {
        Id id = new Id(this, _nextId++);
        EntityLocation location = _emptyArchetype.AddEntity(id, Span<EntityComponentValue>.Empty);
        _entityIndices[id] = location;

        return new Entity(id);
    }

    public void AddComponentToEntity<T>(Id id, T component) where T : notnull
    {
        if (!_entityIndices.TryGetValue(id, out EntityLocation location))
        {
            // todo: handle this scenario instead of failing
            throw new InvalidOperationException($"Entity with Id {id} does not exist in the world.");
        }

        ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();

        EntityType nextEntityType = location.Archetype.EntityType.WithAddedComponent(componentTypeId);

        if (nextEntityType.Equals(location.Archetype.EntityType))
        {
            return;
        }

        Archetype nextArchetype = GetArchetype(nextEntityType);

        EntityLocation nextLocation = nextArchetype.MigrateEntity(location, new EntityComponentValue(componentTypeId, component));

        _entityIndices[id] = nextLocation;
    }



    public void Query<T1>(EntityQueryAction<T1> action) where T1 : notnull
    {
        EntityType entityType = EntityTypeInformation<T1>.EntityType;

        foreach (var archetype in _archetypes)
        {
            if (!archetype.EntityType.HasSubset(entityType))
            {
                continue;
            }

            archetype.Query(action);
        }

    }

    private Archetype GetArchetype(EntityType entityType)
    {
        if (_archetypesByEntityType.TryGetValue(entityType, out Archetype? archetype))
        {
            return archetype;
        }

        archetype = new Archetype(entityType);
        _archetypesByEntityType[entityType] = archetype;
        _archetypes.Add(archetype);

        return archetype;
    }

}
