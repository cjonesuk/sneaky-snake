namespace Axis.ECS;

// todo: Implement object pooling
public sealed class WorldSystemContext
{
    private readonly World _world;
    private readonly float _deltaTime;

    private WorldSystemContext(World world, float deltaTime)
    {
        _world = world;
        _deltaTime = deltaTime;
    }

    public static WorldSystemContext For(World world, float deltaTime)
    {
        return new WorldSystemContext(world, deltaTime);
    }

    public World World => _world;
    public float DeltaTime => _deltaTime;
}

public record struct EntityEventContext(Entity Entity, float DeltaTime);