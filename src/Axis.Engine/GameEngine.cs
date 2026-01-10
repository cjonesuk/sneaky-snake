using Axis.ECS;
using Axis.Engine.Input;
using Axis.Engine.Rendering;
using Raylib_cs;

namespace Axis.Engine;

public record Settings(int ScreenWidth, int ScreenHeight, string Title);

public interface IGameInstance
{
    void OnEngineStart(IGameEngine engine);
    void OnEngineStop(IGameEngine engine);
}

public interface IGameEngine
{
    Settings Settings { get; }
    IDeviceManager Devices { get; }

    void SetWorld(IWorld world);
    void ClearWorld();

}


public sealed class GameEngine : IGameEngine
{
    private readonly IDeviceManager _devices;
    private readonly Settings _settings;
    private readonly WindowRenderTarget _window;

    private IWorld? _world;

    internal GameEngine(IDeviceManager devices, WindowRenderTarget window, Settings settings)
    {
        _devices = devices;
        _window = window;
        _settings = settings;
    }

    public static GameEngine Create(Settings settings)
    {
        var devices = new DeviceManager();
        var window = new WindowRenderTarget(Color.SkyBlue);

        return new GameEngine(devices, window, settings);
    }

    public Settings Settings => _settings;
    public IDeviceManager Devices => _devices;

    public void SetWorld(IWorld world)
    {
        _world = world;
    }

    public void ClearWorld()
    {
        _world = null;
    }

    public void Run(IGameInstance game)
    {
        game.OnEngineStart(this);

        Raylib.InitWindow(_settings.ScreenWidth, _settings.ScreenHeight, _settings.Title);

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            _devices.Poll();

            _world?.ExecuteSystems(deltaTime);

            _window.Render();
        }

        game.OnEngineStop(this);

        Raylib.CloseWindow();
    }

}