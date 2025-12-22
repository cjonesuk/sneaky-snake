using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Input;
using Engine.Rendering;
using Engine.WorldManagement;
using Raylib_cs;

namespace SneakySnake;

public static class StartMenuActions
{
    public sealed class StartGame : InputAction
    {
        public override string Name => "StartGame";
        public static readonly StartGame Instance = new();
    }
}

internal class StartMenuMode : IGameMode, IInputReceiver
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
        _cameraId = _world.SpawnCamera2d(position: new Vector2(400, 300), rotation: 0.0f, zoom: 1.0f, debug: true);
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        _engine.DeviceManager.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Enter, StartMenuActions.StartGame.Instance)
                ]
            )
        ]);

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
        _engine.DeviceManager.KeyboardAndMouse.ClearContext();
    }

    public void Update(float deltaTime)
    {

    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        if (inputEvent.Action is StartMenuActions.StartGame)
        {
            Console.WriteLine("StartGame action received, starting game...");
            _game.StartGame();
        }
    }
}
