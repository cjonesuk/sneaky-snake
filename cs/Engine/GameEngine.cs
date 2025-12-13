using Raylib_cs;

namespace Engine;

public class GameEngine : IGameEngine
{
    private readonly Settings _settings;
    private readonly List<ILayer> _layers = new();

    public GameEngine(Settings settings)
    {
        _settings = settings;
    }

    public Settings Settings => _settings;

    public void ClearLayers()
    {
        _layers.Clear();
    }

    public void SetLayers(IReadOnlyList<ILayer> layers)
    {
        _layers.Clear();
        _layers.AddRange(layers);
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

            foreach (var observer in observers)
            {
                observer.Update(this, deltaTime);
            }

            Raylib.BeginDrawing();

            foreach (var layer in _layers)
            {
                layer.Render();
            }

            Raylib.EndDrawing();
        }

        foreach (var observer in observers)
        {
            observer.OnEngineStop(this);
        }

        Raylib.CloseWindow();
    }
}
