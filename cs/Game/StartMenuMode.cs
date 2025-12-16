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

    public void OnActivate()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        _engine.SpawnText(400, 100, "Welcome to Sneaky Snake!", 32, Color.Black, TextAlignment.Center);
        _engine.SpawnText(400, 150, "Press Enter to Start Game", 24, Color.DarkGray, TextAlignment.Center);
    }

    public void OnDeactivate()
    {
        Console.WriteLine("Disabling Start Menu Mode...");
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
