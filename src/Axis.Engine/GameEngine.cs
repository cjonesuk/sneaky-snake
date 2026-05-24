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
    float DeltaTime { get; }

    void SetWorld(IWorld world);
    void ClearWorld();

    void SetViewports(Viewport[] viewports);

}


public sealed class GameEngine : IGameEngine
{
    private readonly IDeviceManager _devices;
    private readonly Settings _settings;
    private readonly WindowRenderTarget _window;

    private IWorld? _world;
    private float _deltaTime;

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
    public float DeltaTime => _deltaTime;

    public void SetWorld(IWorld world)
    {
        _world = world;
    }

    public void SetViewports(Viewport[] viewports)
    {
        _window.SetViewports(viewports);
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
            _deltaTime = Raylib.GetFrameTime();

            _world?.ClearAllEvents();

            _devices.Poll();

            _world?.ExecuteSystems(_deltaTime);

            _window.Render();
        }

        game.OnEngineStop(this);

        Raylib.CloseWindow();
    }

}