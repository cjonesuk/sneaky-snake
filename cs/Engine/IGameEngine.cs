namespace Engine;

public interface IGameEngine
{
    Settings Settings { get; }

    IEntityComponentManager Entities { get; }

    void SetSystems(IReadOnlyList<ISystem> systems);

    void Run(IReadOnlyList<IGameEngineObserver> observers);
}
