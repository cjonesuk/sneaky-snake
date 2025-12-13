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

        _engine.SpawnFood(2, 3, Color.Orange);
        _engine.SpawnFood(5, 7, Color.Red);
        _engine.SpawnFood(10, 12, Color.Yellow);

        _engine.SpawnText(400, 50, "Game in Progress - Press Enter to End Game", 24, Color.Black, TextAlignment.Center);
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Play Mode...");
        _engine.Entities.RemoveAllEntities();
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
