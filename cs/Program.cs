using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

namespace SneakySnake;

interface IEngine
{
    Settings Settings { get; }

    void SetLayers(IReadOnlyList<ILayer> layers);

    void Run(IReadOnlyList<ISystem> systems);
}

internal record Settings(int ScreenWidth, int ScreenHeight, string Title);

interface ILayer
{
    void Render();
}

internal class BackgroundLayer : ILayer
{
    private readonly IEngine _engine;
    private readonly Color _backgroundColor = Color.SkyBlue;

    public BackgroundLayer(IEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        Raylib.ClearBackground(_backgroundColor);
    }
}

internal class EntityLayer : ILayer
{
    private readonly IEngine _engine;

    public EntityLayer(IEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        Raylib.DrawRectangle(50, 50, 100, 100, Color.Yellow);
    }
}


interface IGameMode
{

}

internal class StartMenuMode : IGameMode
{

}

internal class PlayMode : IGameMode
{

}

interface IGame
{
    // Displays the start menu
    void StartMenu();

    // Starts a new game
    void StartGame();

    // Ends the current game, returning to the main menu
    void EndGame();
}

internal class SneakySnakeGame : IGame
{
    private readonly IEngine _engine;
    private bool _started;

    public SneakySnakeGame(IEngine engine)
    {
        _engine = engine;
        _started = false;
    }

    public void StartMenu()
    {
        Debug.Assert(!_started, "Game is already started.");

        Console.WriteLine("Displaying Start Menu...");

        ILayer[] layers = {
            new BackgroundLayer(_engine),
            new UiLayer(_engine),
        };

        _engine.SetLayers(layers);
    }

    public void StartGame()
    {
        Debug.Assert(!_started, "Game is already started.");
        _started = true;
    }

    public void EndGame()
    {
        Debug.Assert(_started, "Game is not started.");
        _started = false;
    }
}

internal class UiLayer : ILayer
{
    private readonly IEngine _engine;
    private Font _font;

    public UiLayer(IEngine engine)
    {
        _engine = engine;
        _font = Raylib.GetFontDefault();
    }

    public void Render()
    {
        Vector2 textSize = Raylib.MeasureTextEx(_font, "Welcome to Sneaky Snake!", 20, 1);
        Vector2 textPosition = new Vector2((_engine.Settings.ScreenWidth / 2) - (textSize.X / 2), (_engine.Settings.ScreenHeight / 2) - (textSize.Y / 2));

        Raylib.DrawTextEx(_font, "Welcome to Sneaky Snake!", textPosition, 20, 1, Color.Black);
    }
}


interface ISystem
{
    void Attached(IEngine engine);
    void Detached();

    void Update(float deltaTime);
}

internal class GameSystem : ISystem
{
    private IEngine? _engine;
    private IGame? _game;

    public GameSystem()
    {
    }

    public void Attached(IEngine engine)
    {
        Console.WriteLine("GameSystem attached to engine.");
        _engine = engine;
        _game = new SneakySnakeGame(_engine);
        _game.StartMenu();
    }

    public void Detached()
    {
        _engine = null;
        _game = null;
    }

    public void Update(float deltaTime)
    {
        // Game logic update
    }
}

internal class Engine : IEngine
{
    private readonly Settings _settings;
    private readonly List<ILayer> _layers = new();

    public Engine(Settings settings)
    {
        _settings = settings;
    }

    public Settings Settings => _settings;

    public void SetLayers(IReadOnlyList<ILayer> layers)
    {
        _layers.Clear();
        _layers.AddRange(layers);
    }

    public void Run(IReadOnlyList<ISystem> systems)
    {
        Raylib.InitWindow(_settings.ScreenWidth, _settings.ScreenHeight, _settings.Title);

        foreach (var system in systems)
        {
            system.Attached(this);
        }

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            foreach (var system in systems)
            {
                system.Update(deltaTime);
            }

            Raylib.BeginDrawing();

            foreach (var layer in _layers)
            {
                layer.Render();
            }

            Raylib.EndDrawing();
        }

        foreach (var system in systems)
        {
            system.Detached();
        }

        Raylib.CloseWindow();
    }
}

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Settings settings = new Settings(800, 600, "Sneaky Snake");
        IEngine engine = new Engine(settings);

        ISystem[] systems = {
            new GameSystem(),
        };

        engine.Run(systems);
    }
}