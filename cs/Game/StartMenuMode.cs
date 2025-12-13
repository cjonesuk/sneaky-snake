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

        ISystem[] systems = {
             new MenuRenderSystem()
        };

        _engine.SetSystems(systems);

        _engine.Entities.AddEntity(new Transform2d(400, 200), new Text2d(
            "Press Enter to Start Game",
            24,
            Color.Purple,
            TextAlignment.Center
        ));
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Start Menu Mode...");
        _engine.SetSystems(Array.Empty<ISystem>());
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
