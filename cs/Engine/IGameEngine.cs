namespace Engine;

public interface IGameEngine
{
    Settings Settings { get; }

    void ClearLayers();

    void SetLayers(IReadOnlyList<ILayer> layers);

    void Run(IReadOnlyList<IGameEngineObserver> observers);
}
