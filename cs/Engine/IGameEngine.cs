using Engine.Rendering;

namespace Engine;

public interface IGameEngine
{
    Settings Settings { get; }

    void AddWorld(IWorld world);

    void SetViewports(Viewport[] viewports);

    void SetSystems(IReadOnlyList<ISystem> systems);

    void Run(IReadOnlyList<IGameEngineObserver> observers);
}
