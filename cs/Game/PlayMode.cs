using System.Numerics;
using Engine;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

internal class PlayMode : IGameMode
{
    private readonly IGameInstance _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityId _cameraId;
    private readonly EntityId _camera2Id;

    private EntityId _fpsTextEntity;

    public PlayMode(IGameInstance game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = Builders.CreateWorld();
        _cameraId = _world.SpawnCamera2d(rotation: 0.0f, zoom: 0.5f);
        _camera2Id = _world.SpawnCamera2d(rotation: 180.0f, zoom: 0.25f);
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Play Mode...");

        // Register viewport/camera/world with the engine
        _engine.AddWorld(_world);
        _engine.SetViewports(Viewport.SplitColumns(_world, _cameraId, _camera2Id));

        // Spawn entities
        _world.SpawnFood(2, 3, Color.Orange);
        _world.SpawnFood(5, 7, Color.Red);
        _world.SpawnFood(3, 4, Color.Yellow);
        _world.SpawnMovingFood(0, 4, Color.Black);

        _world.SpawnText(new Vector2(400, 50), "Game in Progress - Press Enter to End Game", 24, Color.Black, TextAlignment.Center);

        _fpsTextEntity = _world.SpawnText(new Vector2(10, 10), "FPS: -", 16, Color.Black, TextAlignment.Left);
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
        ref var fpsText = ref _world.Entities.QueryById(_fpsTextEntity).GetRef<Text2d>();
        fpsText.Text = $"FPS: {fps}";
    }
}
