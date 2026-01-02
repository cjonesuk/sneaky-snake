using System.Collections;
using System.Runtime.InteropServices;

namespace AnECS;

public interface IArchetype
{

}

record struct EntityComponentValue(ComponentTypeId ComponentTypeId, object Value);

record struct EntityLocation(IArchetype Archetype, int ComponentIndex);

internal sealed class Archetype : IArchetype
{
    private readonly Dictionary<ComponentTypeId, int> _componentTypeIdToColumnIndex;
    private readonly IList[] _componentColumns;
    private readonly List<Id> _entityIds;
    private readonly EntityType _entityType;


    internal Archetype(EntityType entityType)
    {
        Span<ComponentTypeId> componentTypeIds = entityType.ComponentTypeIds;

        _componentTypeIdToColumnIndex = new Dictionary<ComponentTypeId, int>(componentTypeIds.Length);
        _componentColumns = new IList[componentTypeIds.Length];
        _entityIds = new List<Id>();

        _entityType = entityType;

        for (int index = 0; index < componentTypeIds.Length; index++)
        {
            ComponentTypeId typeId = componentTypeIds[index];
            _componentTypeIdToColumnIndex[typeId] = index;

            Type componentType = ComponentTypeRegistry.GetTypeById(typeId);
            Type listType = typeof(List<>).MakeGenericType(componentType);

            _componentColumns[index] = (IList)(Activator.CreateInstance(listType) ?? throw new InvalidOperationException($"Could not create list of type {listType}"));
        }
    }

    public EntityLocation AddEntity(SparseEntity sparseEntity)
    {
        _entityIds.Add(sparseEntity.Id);
        int entityIndex = _entityIds.Count - 1;

        foreach (var component in sparseEntity.Components)
        {
            int columnIndex = _componentTypeIdToColumnIndex[component.Key];
            _componentColumns[columnIndex].Add(component.Value);
        }

        return new EntityLocation(this, entityIndex);
    }
}

public interface IWorld
{
    void AddComponentToEntity<T>(Id id, T component) where T : notnull;
}

internal sealed class World : IWorld
{
    private uint _nextId = 1;
    private readonly Dictionary<Id, EntityLocation> _entityIndices;
    private readonly Dictionary<Id, SparseEntity> _sparseEntities;
    private readonly Dictionary<EntityType, Archetype> _archetypes;

    internal World()
    {
        _entityIndices = new Dictionary<Id, EntityLocation>();
        _sparseEntities = new Dictionary<Id, SparseEntity>();
        _archetypes = new Dictionary<EntityType, Archetype>();
    }

    public static World Create()
    {
        return new World();
    }

    public Entity Entity()
    {
        Id id = new Id(this, _nextId++);
        _sparseEntities[id] = new SparseEntity(id);
        return new Entity(id);
    }

    public void AddComponentToEntity<T>(Id id, T component) where T : notnull
    {
        if (_entityIndices.TryGetValue(id, out EntityLocation location))
        {
            // todo: move the entity to a new archetype
            throw new NotImplementedException();
        }

        if (_sparseEntities.TryGetValue(id, out SparseEntity? sparseEntity))
        {
            ComponentTypeId componentTypeId = ComponentTypeRegistry.GetComponentTypeId<T>();
            sparseEntity.AddComponent(componentTypeId, component);
        }

        // todo: handle this scenario instead of failing
        throw new InvalidOperationException($"Entity with Id {id} does not exist in the world.");
    }

    public void Apply()
    {
        foreach (var sparseEntity in _sparseEntities.Values)
        {
            EntityType entityType = sparseEntity.DetermineType();

            Archetype archetype = GetArchetype(entityType);

            EntityLocation location = archetype.AddEntity(sparseEntity);
            _entityIndices[sparseEntity.Id] = location;
        }

        _sparseEntities.Clear();
    }

    private Archetype GetArchetype(EntityType entityType)
    {
        if (_archetypes.TryGetValue(entityType, out Archetype? archetype))
        {
            return archetype;
        }

        archetype = new Archetype(entityType);
        _archetypes[entityType] = archetype;

        return archetype;
    }
}
