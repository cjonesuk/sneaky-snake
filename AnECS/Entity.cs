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
    public static void Set<T>(this Entity entity, T component) where T : notnull
    {
        entity.World.SetComponentOnEntity(entity.Id, component);
    }
}
