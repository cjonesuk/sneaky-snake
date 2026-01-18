using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Collision;
using Axis.Engine.Components;
using Axis.Engine.Input;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;


internal sealed class EntityInputReceiver : IInputReceiver
{
    private Entity _entity;

    public EntityInputReceiver()
    {
        _entity = Entity.Invalid;
    }

    public EntityInputReceiver(Entity entity)
    {
        _entity = entity;
    }

    public void SetEntity(Entity entity)
    {
        // Note: This method allows changing the entity associated with this receiver.
        // Ensure that the new entity is valid and has the necessary components to handle input events.
        _entity = entity;
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        if (!_entity.IsValid())
        {
            return;
        }

        _entity.World.Events.AddEvent(_entity.Id, ref inputEvent);
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
    private readonly EntityInputReceiver _player1InputReceiver;
    private readonly EntityInputReceiver _player2InputReceiver;

    public PlayGameMode(IPingPongGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;
        _player1InputReceiver = new EntityInputReceiver(_player1Paddle);
        _player2InputReceiver = new EntityInputReceiver(_player2Paddle);
    }

    public void Activate()
    {
        _game.World.RemoveAllEntities();
        _game.World.RemoveAllSystems();

        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);
        _ball = _world.SpawnBall(new Vector2(400, 300), Color.Red);
        _player1Paddle = _world.SpawnPaddle(1, new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(2, new Vector2(750, 300), Color.Green);

        _player1InputReceiver.SetEntity(_player1Paddle);
        _player2InputReceiver.SetEntity(_player2Paddle);

        var globalContext = new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayGameActions.QuitGame)
                ]
            );

        var player1Context = new KeyboardInputContext(
                _player1InputReceiver,
                keyDown: [
                    new KeyboardInputMapping(KeyboardKey.W, PlayGameActions.MovePaddleUp),
                    new KeyboardInputMapping(KeyboardKey.S, PlayGameActions.MovePaddleDown)
                ],
                keyPressed: []
            );

        var player2Context = new KeyboardInputContext(
                _player2InputReceiver,
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
            .System<PossessedByPlayer, Paddle>()
            .ForEach(static (ref context, ref iter, ref possessed, ref paddle) =>
            {
                if (!context.World.Events.TryGetEventStream<InputEvent>(iter.Id, out var inputBuffer))
                {
                    return;
                }

                float deltaTime = context.DeltaTime;
                paddle.Speed = 0;

                foreach (var inputEvent in inputBuffer.AsSpan())
                {
                    if (inputEvent.Id == PlayGameActions.MovePaddleUp)
                    {
                        paddle.Speed = -paddle.MaxSpeed;
                    }
                    else if (inputEvent.Id == PlayGameActions.MovePaddleDown)
                    {
                        paddle.Speed = paddle.MaxSpeed;
                    }
                }
            });

        _world
            .System<Transform2d, Paddle>()
            .ForEach(static (ref context, ref iter, ref transform, ref paddle) =>
            {
                float deltaTime = context.DeltaTime;

                transform.Position.Y += paddle.Speed * deltaTime;
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

        _world
            .System<Transform2d, Ball, BasicShape>()
            .ForEach((ref context, ref iter, ref transform, ref ball, ref shape) =>
            {
                float deltaTime = context.DeltaTime;
                transform.Position += ball.Direction * ball.Speed * deltaTime;

                // Bounce off top and bottom of screen
                var topY = transform.Position.Y - shape.HalfExtents.Y;
                var bottomY = transform.Position.Y + shape.HalfExtents.Y;
                var leftX = transform.Position.X - shape.HalfExtents.X;
                var rightX = transform.Position.X + shape.HalfExtents.X;

                if (topY < 0 || bottomY > 600)
                {
                    ball.Direction.Y = -ball.Direction.Y;
                }

                // Bounce off screen bounds
                if (leftX < 0 || rightX > 800)
                {
                    ball.Direction.X = -ball.Direction.X;
                }
            });

        _world.AddSystem(new CollisionSystem());
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
