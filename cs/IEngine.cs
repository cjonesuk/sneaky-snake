namespace SneakySnake;

interface IEngine
{
    Settings Settings { get; }

    void ClearLayers();

    void SetLayers(IReadOnlyList<ILayer> layers);

    void Run(IReadOnlyList<ISystem> systems);
}
