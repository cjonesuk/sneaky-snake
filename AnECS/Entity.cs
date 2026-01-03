namespace AnECS;

public readonly struct Entity
{
    private readonly Id _id;

    public Id Id => _id;

    public IWorld World => _id.World;

    public Entity(Id id)
    {
        _id = id;
    }
}

public static class EntityWorldExtensions
{
    public static void Set<T>(this Entity entity, T component) where T : struct
    {
        entity.World.SetComponentOnEntity(entity.Id, component);
    }

    public static void Add<T>(this Entity entity) where T : struct
    {
        entity.World.AddComponentToEntity<T>(entity.Id);
    }

    public static bool Has<T>(this Entity entity) where T : struct
    {
        return entity.World.EntityHasComponent<T>(entity.Id);
    }

    public static void Remove<T>(this Entity entity) where T : struct
    {
        entity.World.RemoveComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Get a mutable reference to the component T from the entity.
    /// </summary>
    public static ref T GetRef<T>(this Entity entity) where T : struct
    {
        return ref entity.World.GetComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Gets a copy of the component T from the entity.
    /// </summary>
    public static T Get<T>(this Entity entity) where T : struct
    {
        return entity.World.GetComponentFromEntity<T>(entity.Id);
    }
}
