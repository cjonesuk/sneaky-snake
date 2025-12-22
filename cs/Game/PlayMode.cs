using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Input;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

public static class PlayActions
{
    public sealed class EndGameAction : InputAction
    {
        public override string Name => "EndGame";
        public static readonly EndGameAction Instance = new();
    }
}

internal class PlayMode : IGameMode, IInputReceiver
{
    private readonly IGameInstance _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityId _cameraId;

    private EntityId _fpsTextEntity;


    public PlayMode(IGameInstance game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = Builders.CreateWorld();
        _cameraId = _world.SpawnCamera2d(position: new Vector2(400, 300), rotation: 0.0f, zoom: 1f);
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Play Mode...");

        _engine.DeviceManager.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayActions.EndGameAction.Instance)
                ]
            )
        ]);

        _engine.AddWorld(_world);

        _engine.SetViewports([Viewport.Fullscreen(_world, _cameraId)]);

        // Spawn entities
        _world.SpawnFood(2, 3, Color.Orange);
        _world.SpawnFood(5, 7, Color.Red);
        _world.SpawnFood(3, 4, Color.Yellow);
        _world.SpawnMovingFood(0, 4, Color.Black);

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
        if (inputEvent.Action is PlayActions.EndGameAction)
        {
            Console.WriteLine("EndGame action received, ending game...");
            _game.EndGame();
        }
    }
}
