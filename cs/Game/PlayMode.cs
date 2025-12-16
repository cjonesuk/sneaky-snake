using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class PlayMode : IGameMode
{
    private readonly IGame _game;
    private readonly IGameEngine _engine;

    private EntityId _fpsTextEntity;

    public PlayMode(IGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Play Mode...");

        _engine.SpawnFood(2, 3, Color.Orange);
        _engine.SpawnFood(5, 7, Color.Red);
        _engine.SpawnFood(3, 4, Color.Yellow);
        _engine.SpawnMovingFood(0, 4, Color.Black);

        _engine.SpawnText(400, 50, "Game in Progress - Press Enter to End Game", 24, Color.Black, TextAlignment.Center);

        _fpsTextEntity = _engine.SpawnText(10, 10, "FPS: -", 16, Color.Black, TextAlignment.Left);
    }

    public void OnDeactivate()
    {
        Console.WriteLine("Disabling Play Mode...");
    }

    public void Update(float deltaTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Console.WriteLine("Enter key pressed, ending game...");
            _game.EndGame();
        }

        int fps = Raylib.GetFPS();
        ref var fpsText = ref _engine.Entities.QueryById(_fpsTextEntity).GetRef<Text2d>();
        fpsText.Text = $"FPS: {fps}";
    }
}
