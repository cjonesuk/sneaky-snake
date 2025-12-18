using Engine;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

internal class StartMenuMode : IGameMode
{
    private readonly IGameInstance _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityId _cameraId;

    public StartMenuMode(IGameInstance game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = Builders.CreateWorld();
        _cameraId = _world.SpawnCamera2d();
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        _engine.AddWorld(_world);
        _engine.SetViewports(
        [
            new Viewport(0, 0, 1.0f, 1.0f, _world, _cameraId, Color.Red)
        ]);

        // Spawn entities
        _world.SpawnText(400, 100, "Welcome to Sneaky Snake!", 32, Color.Black, TextAlignment.Center);
        _world.SpawnText(400, 150, "Press Enter to Start Game", 24, Color.DarkGray, TextAlignment.Center);
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
