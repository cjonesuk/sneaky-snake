using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Collision;
using Axis.Engine.Input;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;

internal sealed class PlayGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private Entity _camera;
    private Entity _ball;
    private Entity _player1Paddle;
    private Entity _player2Paddle;
    private Entity _player1Goal;
    private Entity _player2Goal;
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
        _ball = _world.SpawnBall(new Vector2(400, 300), new Vector2(1, 0), Color.Red);
        _player1Paddle = _world.SpawnPaddle(1, new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(2, new Vector2(750, 300), Color.Green);
        _player1Goal = _world.SpawnGoal(1, new Vector2(0, 300), new Vector2(10, 300));
        _player2Goal = _world.SpawnGoal(2, new Vector2(800, 300), new Vector2(10, 300));

        _world.SpawnWall(new Vector2(400, 0), new Vector2(400, 10));
        _world.SpawnWall(new Vector2(400, 600), new Vector2(400, 10));

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
            .System<Transform2d, Paddle>()
            .ForEach(static (ref context, ref iter, ref transform, ref paddle) =>
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

                transform.Position.Y += paddle.Speed * deltaTime;
            });

        _world
            .System<Transform2d, Ball>()
            .ForEach(static (ref context, ref iter, ref transform, ref ball) =>
            {
                // Move the ball
                float deltaTime = context.DeltaTime;
                transform.Position += ball.Direction * ball.Speed * deltaTime;
            });

        _world.AddSystem(new CollisionSystem());

        _world
            .System<Transform2d, Paddle, CollisionBody>()
            .ForEach(static (ref context, ref iter, ref transform, ref paddle, ref body) =>
            {
                // Dont let paddles go past the walls
                var collisions = context.World.Events.GetEventStream<CollisionWithEvent>(iter.Id);

                foreach (var ev in collisions.AsSpan())
                {
                    var wallEntity = context.World.GetEntity(ev.EntityId);

                    if (!wallEntity.Has<Wall>())
                    {
                        continue;
                    }

                    ref var wall = ref wallEntity.GetRef<Wall>();
                    ref var wallTransform = ref wallEntity.GetRef<Transform2d>();
                    ref var wallBody = ref wallEntity.GetRef<CollisionBody>();

                    // Simple collision response: stop the paddle from moving further
                    if (paddle.Speed < 0)
                    {
                        // Moving up
                        transform.Position.Y = wallTransform.Position.Y + wallBody.HalfExtents.Y + body.HalfExtents.Y;
                    }
                    else if (paddle.Speed > 0)
                    {
                        // Moving down
                        transform.Position.Y = wallTransform.Position.Y - wallBody.HalfExtents.Y - body.HalfExtents.Y;
                    }
                }
            });

        _world
            .System<Transform2d, Ball, CollisionBody>()
            .ForEach(static (ref context, ref iter, ref transform, ref ball, ref body) =>
            {
                var collisionStarted = context.World.Events.GetEventStream<CollisionStartedWithEvent>(iter.Id);

                foreach (var ev in collisionStarted.AsSpan())
                {
                    var entity = context.World.GetEntity(ev.EntityId);

                    if (entity.Has<Paddle>())
                    {
                        // Bounce off paddle, using the paddle's position to influence direction
                        ref var paddleTransform = ref entity.GetRef<Transform2d>();
                        float relativeIntersectY = (paddleTransform.Position.Y + 0) - (transform.Position.Y + 0);
                        float normalizedRelativeIntersectionY = (relativeIntersectY / (body.HalfExtents.Y + entity.GetRef<CollisionBody>().HalfExtents.Y));
                        float bounceAngle = normalizedRelativeIntersectionY * (5 * (float)(Math.PI / 12));

                        ball.Direction.X = MathF.Cos(bounceAngle) * (ball.Direction.X < 0 ? 1 : -1);
                        ball.Direction.Y = -MathF.Sin(bounceAngle);
                        ball.Direction = Vector2.Normalize(ball.Direction);
                    }
                    else if (entity.Has<Wall>())
                    {
                        // Bounce off wall
                        ball.Direction.Y = -ball.Direction.Y;
                    }
                }
            });

        _world
            .System<Goal>()
            .ForEach(static (ref context, ref iter, ref goal) =>
            {
                var collisionStarted = context.World.Events.GetEventStream<CollisionStartedWithEvent>(iter.Id);

                foreach (var ev in collisionStarted.AsSpan())
                {
                    var entity = context.World.GetEntity(ev.EntityId);
                    if (!entity.Has<Ball>())
                    {
                        continue;
                    }

                    Console.WriteLine($"Player {goal.PlayerNumber} scored!");
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
