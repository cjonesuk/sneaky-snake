namespace Axis.ECS;

public sealed class ExportedEntity
{
    private readonly IWorld _world;
    private readonly Id _id;
    private readonly EntityType _entityType;
    private readonly Dictionary<Id, object> _components;

    public IWorld World => _world;
    public Id Id => _id;
    public EntityType EntityType => _entityType;
    public IReadOnlyDictionary<Id, object> Components => _components;

    internal ExportedEntity(Id id, IWorld world, EntityType entityType, Dictionary<Id, object> components)
    {
        _id = id;
        _world = world;
        _entityType = entityType;
        _components = components;
    }

    public static ExportedEntity Create(IWorld world, Id id)
    {
        EntityType entityType = world.GetEntityType(id);

        var components = new Dictionary<Id, object>();

        foreach (Id componentId in entityType.ComponentIds)
        {
            var componentMetadata = world.Components.GetMetadata(componentId);
            var componentType = componentMetadata.ComponentType;
            var getComponentMethod = typeof(IWorld).GetMethod("GetComponentFromEntity")?.MakeGenericMethod(componentType);
            if (getComponentMethod == null)
            {
                throw new InvalidOperationException($"Could not find method GetComponentFromEntity for type {componentType}");
            }

            object component = getComponentMethod.Invoke(world, [id]) ?? throw new InvalidOperationException($"Could not get component of type {componentType} from entity {id}");

            components[componentId] = component;
        }

        return new ExportedEntity(id, world, entityType, components);
    }

    public int ComponentCount => _components.Count;

    public T GetComponent<T>() where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();
        return (T)_components[componentId];
    }
}
