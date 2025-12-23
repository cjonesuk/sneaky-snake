using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Input;
using Engine.Rendering;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
using Raylib_cs;

namespace SneakySnake;

internal class PlayMode : IGameMode, IInputReceiver
{
    private readonly IGameInstance _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityId _cameraId;
    private readonly SnakeActor _snake;

    private EntityId _fpsTextEntity;


    public PlayMode(IGameInstance game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = Builders.CreateWorld();
        _cameraId = _world.SpawnCamera2d(position: new Vector2(400, 300), rotation: 0.0f, zoom: 1f);
        _snake = _world.SpawnSnake(new Vector2(400, 300));
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Play Mode...");

        _engine.DeviceManager.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayActions.EndGame.Instance)
                ]
            ),
            new KeyboardInputContext(
                _snake,
                keyDown: [
                    new KeyboardInputMapping(KeyboardKey.W, SnakeActions.MoveForward.Instance),
                    new KeyboardInputMapping(KeyboardKey.S, SnakeActions.SlowDown.Instance),
                    new KeyboardInputMapping(KeyboardKey.A, SnakeActions.TurnLeft.Instance),
                    new KeyboardInputMapping(KeyboardKey.D, SnakeActions.TurnRight.Instance)
                ],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Space, SnakeActions.GenericAction.Instance)
                ]
            )
        ]);

        _engine.AddWorld(_world);

        _engine.SetViewports([Viewport.Fullscreen(_world, _cameraId)]);

        // Spawn entities
        _world.SpawnFood(new Vector2(100, 75), Color.Orange);

        _world.SpawnText(new Vector2(400, 50), "Game in Progress - Press Q to End Game", 24, Color.Black, TextAlignment.Center);

        _fpsTextEntity = _world.SpawnText(new Vector2(10, 10), "FPS: -", 32, Color.Black, TextAlignment.Left);
    }

    public void OnDeactivate()
    {
        Console.WriteLine("Disabling Play Mode...");
        _engine.DeviceManager.KeyboardAndMouse.ClearContext();
    }

    public void Update(float deltaTime)
    {
        int fps = Raylib.GetFPS();
        ref var fpsText = ref _world.Entities.QueryById(_fpsTextEntity).GetRef<Text2d>();
        fpsText.Text = $"FPS: {fps}";
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        if (inputEvent.Action is PlayActions.EndGame)
        {
            Console.WriteLine("EndGame action received, ending game...");
            _game.EndGame();
        }
    }
}
