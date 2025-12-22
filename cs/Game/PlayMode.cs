using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Input;
using Engine.Rendering;
using Raylib_cs;

namespace SneakySnake;

public static class PlayActions
{
    public sealed class EndGame : InputAction
    {
        public override string Name => "EndGame";
        public static readonly EndGame Instance = new();
    }
}

public static class SnakeActions
{
    public sealed class MoveForward : InputAction
    {
        public override string Name => "MoveForward";
        public static readonly MoveForward Instance = new();
    }

    public sealed class SlowDown : InputAction
    {
        public override string Name => "SlowDown";
        public static readonly SlowDown Instance = new();
    }

    public sealed class TurnLeft : InputAction
    {
        public override string Name => "TurnLeft";
        public static readonly TurnLeft Instance = new();
    }

    public sealed class TurnRight : InputAction
    {
        public override string Name => "TurnRight";
        public static readonly TurnRight Instance = new();
    }
}


internal struct SnakeControl
{
    public readonly List<InputAction> PendingActions;
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float MaxTurnRate;
    public float Speed;

    public SnakeControl(
        float maxSpeed,
        float acceleration,
        float deceleration,
        float maxTurnRate)
    {
        PendingActions = new List<InputAction>();
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        Deceleration = deceleration;
        MaxTurnRate = maxTurnRate;
        Speed = 0f;
    }
}

internal sealed class SnakeControlInputReceiver : IInputReceiver
{
    private readonly EntityId _entityId;
    private readonly IWorld _world;

    public SnakeControlInputReceiver(EntityId entityId, IWorld world)
    {
        _entityId = entityId;
        _world = world;
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        var entity = _world.Entities.QueryById(_entityId);

        entity.GetRef<SnakeControl>().PendingActions.Add(inputEvent.Action);
    }
}

internal sealed class SnakeControlSystem : IWorldSystem
{
    private readonly float _friction = 5f;

    public void Update(IWorld world, float deltaTime)
    {
        var result = world.Entities.QueryAll<Transform2d, SnakeControl>();

        foreach (var (transforms, controls) in result)
        {
            for (int index = 0; index < transforms.Count; index++)
            {
                var transform = transforms[index];
                var control = controls[index];

                control.Speed = MathF.Max(0, control.Speed - _friction * deltaTime);

                foreach (var action in control.PendingActions)
                {
                    if (action is SnakeActions.MoveForward)
                    {
                        control.Speed = MathF.Min(control.MaxSpeed, control.Speed + control.Acceleration * deltaTime);
                    }
                    else if (action is SnakeActions.SlowDown)
                    {
                        control.Speed = MathF.Max(0, control.Speed - control.Deceleration * deltaTime);
                    }
                    else if (action is SnakeActions.TurnLeft)
                    {
                        transform.Rotation -= control.MaxTurnRate * deltaTime;
                    }
                    else if (action is SnakeActions.TurnRight)
                    {
                        transform.Rotation += control.MaxTurnRate * deltaTime;
                    }
                }

                // Update position based on speed and rotation
                float radians = MathF.PI / 180f * transform.Rotation;
                transform.Position += new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * control.Speed * deltaTime;

                // Clear pending actions after processing
                control.PendingActions.Clear();

                transforms[index] = transform;
                controls[index] = control;
            }
        }

    }
}

internal class PlayMode : IGameMode, IInputReceiver
{
    private readonly IGameInstance _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityId _cameraId;
    private readonly EntityId _snakeEntityId;

    private EntityId _fpsTextEntity;


    public PlayMode(IGameInstance game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = Builders.CreateWorld();
        _cameraId = _world.SpawnCamera2d(position: new Vector2(400, 300), rotation: 0.0f, zoom: 1f);
        _snakeEntityId = _world.SpawnSnake(new Vector2(400, 300));
    }

    public void OnActivate()
    {
        Console.WriteLine("Starting Play Mode...");

        var snakeInput = new SnakeControlInputReceiver(_snakeEntityId, _world);

        _engine.DeviceManager.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayActions.EndGame.Instance)
                ]
            ),
            new KeyboardInputContext(
                snakeInput,
                keyDown: [
                    new KeyboardInputMapping(KeyboardKey.W, SnakeActions.MoveForward.Instance),
                    new KeyboardInputMapping(KeyboardKey.S, SnakeActions.SlowDown.Instance),
                    new KeyboardInputMapping(KeyboardKey.A, SnakeActions.TurnLeft.Instance),
                    new KeyboardInputMapping(KeyboardKey.D, SnakeActions.TurnRight.Instance)
                ],
                keyPressed: []
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
