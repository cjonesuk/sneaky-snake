namespace Engine;

public interface IGameEngine
{
    Settings Settings { get; }

    IEntityComponentManager Entities { get; }

    void ClearLayers();

    void SetLayers(IReadOnlyList<ILayer> layers);

    void Run(IReadOnlyList<IGameEngineObserver> observers);
}
