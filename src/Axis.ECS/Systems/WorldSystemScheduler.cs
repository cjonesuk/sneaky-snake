using System.Runtime.InteropServices;

namespace Axis.ECS;

public sealed class WorldSystemScheduler
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

    public void RemoveAllSystems()
    {
        _systems.Clear();
    }

    public Span<IWorldSystem> GetSystems()
    {
        return CollectionsMarshal.AsSpan(_systems);
    }
}
