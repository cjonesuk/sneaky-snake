using Engine.Rendering;
using Raylib_cs;

namespace Engine;

public class GameEngine : IGameEngine
{
    private readonly Settings _settings;
    private readonly List<ISystem> _systems = new();
    private readonly List<IWorld> _worlds = new();
    private readonly WindowRenderTarget _window = new(Color.Black);

    public GameEngine(Settings settings)
    {
        _settings = settings;
    }

    public Settings Settings => _settings;

    public void SetSystems(IReadOnlyList<ISystem> systems)
    {
        _systems.Clear();
        _systems.AddRange(systems);

        foreach (var system in _systems)
        {
            system.Attached(this);
        }
    }

    public void AddWorld(IWorld world)
    {
        _worlds.Add(world);
    }

    public void SetViewports(Viewport[] viewports)
    {
        _window.SetViewports(viewports);
    }

    public void Run(IReadOnlyList<IGameEngineObserver> observers)
    {
        Raylib.InitWindow(_settings.ScreenWidth, _settings.ScreenHeight, _settings.Title);

        foreach (var observer in observers)
        {
            observer.OnEngineStart(this);
        }

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            foreach (var world in _worlds)
            {
                world.Tick(deltaTime);
            }

            foreach (var observer in observers)
            {
                observer.Update(this, deltaTime);
            }

            _window.Render();

        }

        foreach (var observer in observers)
        {
            observer.OnEngineStop(this);
        }

        Raylib.CloseWindow();
    }
}
