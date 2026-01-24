using System.Runtime.CompilerServices;

namespace Axis.ECS;

public readonly struct Entity
{
    private readonly IWorld _world;
    private readonly Id _id;

    public static readonly Entity Invalid = new Entity(null!, Id.Invalid);

    public Id Id => _id;

    public IWorld World => _world;

    private Entity(IWorld world, Id id)
    {
        _world = world;
        _id = id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Entity Create(IWorld world, Id id)
    {
        return new Entity(world, id);
    }

    public bool IsValid()
    {
        return _world != null && _id.IsValid();
    }
}

public static class EntityWorldExtensions
{
    public static void Set<T>(this Entity entity, T component) where T : unmanaged
    {
        Id componentId = entity.World.Components.GetId<T>();
        entity.World.SetComponentOnEntity(entity.Id, componentId, ref component);
    }

    public static void SetPair<T>(this Entity entity, T component, Entity target) where T : unmanaged
    {
        entity.World.SetRelationshipOnEntity(entity.Id, target.Id, ref component);
    }

    public static void Add<T>(this Entity entity) where T : unmanaged
    {
        entity.World.AddComponentToEntity<T>(entity.Id);
    }

    public static bool Has<T>(this Entity entity) where T : unmanaged
    {
        return entity.World.EntityHasComponent<T>(entity.Id);
    }

    public static void Remove<T>(this Entity entity) where T : unmanaged
    {
        entity.World.RemoveComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Get a mutable reference to the component T from the entity.
    /// </summary>
    public static ref T GetRef<T>(this Entity entity) where T : unmanaged
    {
        return ref entity.World.GetComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Gets a copy of the component T from the entity.
    /// </summary>
    public static T Get<T>(this Entity entity) where T : unmanaged
    {
        return entity.World.GetComponentFromEntity<T>(entity.Id);
    }

    public static bool IsAlive(this ref Entity entity)
    {
        return entity.World.IsEntityAlive(entity.Id);
    }

    public static void Delete(this Entity entity)
    {
        entity.World.RemoveEntity(entity.Id);
    }

    public static ExportedEntity DebugExport(this Entity entity)
    {
        return ExportedEntity.Create(entity.World, entity.Id);
    }

    public static void AddEvent<TEvent>(this Entity entity, ref TEvent @event)
    {
        entity.World.Events.AddEvent(entity.Id, @event);
    }
}

