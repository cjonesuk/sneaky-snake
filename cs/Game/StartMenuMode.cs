using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class StartMenuMode : IGameMode
{
    private readonly IGame _game;
    private readonly IGameEngine _engine;

    public StartMenuMode(IGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
    }

    public void Enable()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        ILayer[] layers = {
            new BackgroundLayer(_engine, Color.SkyBlue),
            new UiLayer(_engine),
        };

        _engine.SetLayers(layers);
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Start Menu Mode...");
        _engine.ClearLayers();
    }

    public void Update(float deltaTime)
    {
        // Update logic for start menu
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Console.WriteLine("Enter key pressed, starting game...");
            _game.StartGame();
        }
    }
}
