using Engine.Input;
using Engine.Rendering;
using Engine.WorldManagement;

namespace Engine;

public interface IGameEngine
{
    Settings Settings { get; }

    IDeviceManager DeviceManager { get; }

    void AddWorld(IWorld world);

    void SetViewports(Viewport[] viewports);

    void Run(IReadOnlyList<IGameEngineObserver> observers);
}
