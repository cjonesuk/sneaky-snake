namespace AnECS;

internal sealed class WorldSystemScheduler
{
    private readonly List<IWorldSystem> _systems;

    public WorldSystemScheduler()
    {
        _systems = new List<IWorldSystem>();
    }

    public void AddSystem(IWorldSystem system)
    {
        _systems.Add(system);
    }

    public void ExecuteAll(ref WorldSystemData data)
    {
        foreach (var system in _systems)
        {
            system.Execute(ref data);
        }
    }
}
