using System.Numerics;
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
        _cameraId = _world.SpawnCamera2d(position: new Vector2(000, 000), rotation: 0.0f, zoom: 1.0f, debug: true);
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        _engine.AddWorld(_world);
        _engine.SetViewports(
        [
            Viewport.Fullscreen(_world, _cameraId)
        ]);

        // Spawn entities
        _world.SpawnText(new Vector2(400, 100), "Welcome to Sneaky Snake!", 32, Color.Black, TextAlignment.Center);
        _world.SpawnText(new Vector2(400, 150), "Press Enter to Start Game", 24, Color.DarkGray, TextAlignment.Center);
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
