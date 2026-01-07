namespace Axis.ECS;

internal readonly struct WorldDeferredCommandsScope : IDisposable
{
    private readonly World _world;

    public WorldDeferredCommandsScope(World world)
    {
        _world = world;
    }

    public void Dispose()
    {
        _world.EndDeferringCommands();
    }
}
