using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class PlayMode : IGameMode
{
    private readonly IGame _game;
    private readonly IGameEngine _engine;

    public PlayMode(IGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
    }

    public void Enable()
    {
        Console.WriteLine("Starting Play Mode...");
        ILayer[] layers = {
            new BackgroundLayer(_engine, Color.Lime),
        };
        _engine.SetLayers(layers);
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Play Mode...");
        _engine.ClearLayers();
    }

    public void Update(float deltaTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Console.WriteLine("Enter key pressed, ending game...");
            _game.EndGame();
        }
    }
}
