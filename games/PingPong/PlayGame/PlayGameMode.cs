using System.Numerics;
using System.Runtime.InteropServices;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Components;
using Axis.Engine.Input;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;

internal sealed class PlayerInputBuffer : IInputReceiver
{
    private readonly int _playerNumber;
    private readonly List<InputEvent> _inputEvents;

    public PlayerInputBuffer(int playerNumber)
    {
        _playerNumber = playerNumber;
        _inputEvents = new List<InputEvent>();
    }

    public Span<InputEvent> GetEvents()
    {
        return CollectionsMarshal.AsSpan(_inputEvents);
    }

    public void ClearEvents()
    {
        _inputEvents.Clear();
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        Console.WriteLine($"Player {_playerNumber} received input: {inputEvent.Id}");
        _inputEvents.Add(inputEvent);
    }
}

internal sealed class PlayGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private Entity _camera;
    private Entity _ball;
    private Entity _player1Paddle;
    private Entity _player2Paddle;
    private readonly Dictionary<int, PlayerInputBuffer> _playerInputBuffers;



    public PlayGameMode(IPingPongGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;

        _playerInputBuffers = new Dictionary<int, PlayerInputBuffer>();
    }

    public void Activate()
    {
        _game.World.RemoveAllEntities();
        _game.World.RemoveAllSystems();

        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);
        _ball = _world.SpawnBall(new Vector2(400, 300), Color.Red);
        _player1Paddle = _world.SpawnPaddle(1, new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(2, new Vector2(750, 300), Color.Green);

        _playerInputBuffers.Clear();
        _playerInputBuffers[1] = new PlayerInputBuffer(1);
        _playerInputBuffers[2] = new PlayerInputBuffer(2);

        var globalContext = new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayGameActions.QuitGame)
                ]
            );

        var player1Context = new KeyboardInputContext(
                _playerInputBuffers[1],
                keyDown: [
                    new KeyboardInputMapping(KeyboardKey.W, PlayGameActions.MovePaddleUp),
                    new KeyboardInputMapping(KeyboardKey.S, PlayGameActions.MovePaddleDown)
                ],
                keyPressed: []
            );

        var player2Context = new KeyboardInputContext(
                _playerInputBuffers[2],
                keyDown: [
                    new KeyboardInputMapping(KeyboardKey.Up, PlayGameActions.MovePaddleUp),
                    new KeyboardInputMapping(KeyboardKey.Down, PlayGameActions.MovePaddleDown)
                ],
                keyPressed: []
            );

        _engine.Devices.KeyboardAndMouse.BindContext([
            globalContext,
            player1Context,
            player2Context
        ]);

        _game.SetCamera(_camera);

        SetupSystems();

        Console.WriteLine("Start Menu Activated");
    }

    private void SetupSystems()
    {
        _world
            .System<PossessedByPlayer, Transform2d, Paddle>()
            .ForEach((ref context, ref iter, ref possessed, ref transform, ref paddle) =>
            {
                float deltaTime = context.DeltaTime;
                var inputBuffer = _playerInputBuffers[possessed.PlayerNumber];

                foreach (var inputEvent in inputBuffer.GetEvents())
                {
                    if (inputEvent.Id == PlayGameActions.MovePaddleUp)
                    {
                        transform.Position.Y -= paddle.MaxSpeed * deltaTime;
                    }
                    else if (inputEvent.Id == PlayGameActions.MovePaddleDown)
                    {
                        transform.Position.Y += paddle.MaxSpeed * deltaTime;
                    }
                }

                inputBuffer.ClearEvents();
            });

        _world
            .System<Transform2d, Paddle, BasicShape>()
            .ForEach((ref context, ref iter, ref transform, ref paddle, ref shape) =>
            {
                // Dont let paddles go off screen
                var topY = transform.Position.Y - shape.HalfExtents.Y;
                var bottomY = transform.Position.Y + shape.HalfExtents.Y;

                if (topY < 0)
                {
                    transform.Position.Y = shape.HalfExtents.Y;
                }
                else if (bottomY > 600)
                {
                    transform.Position.Y = 600 - shape.HalfExtents.Y;
                }
            });

    }


    public void Deactivate()
    {
        _engine.Devices.KeyboardAndMouse.ClearContext();

        Console.WriteLine("Start Menu Deactivated");
    }

    void IInputReceiver.ReceiveInput(InputEvent inputEvent)
    {
        if (inputEvent.Id == PlayGameActions.QuitGame)
        {
            Console.WriteLine("Quit Game action received!");
            _game.QuitGame();
        }
    }
}
