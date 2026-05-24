using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine;
using Axis.Engine.Collision;
using Axis.Engine.Input;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;

internal enum Player
{
    Player1 = 1,
    Player2 = 2,
}

internal sealed class PlayGameMode : IGameMode, IInputReceiver, IWorldSystem
{
    private const int MaxScore = 3;

    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private Entity _camera;
    private Entity _ball;
    private Entity _player1Paddle;
    private Entity _player2Paddle;
    private readonly EntityInputReceiver _player1InputReceiver;
    private readonly EntityInputReceiver _player2InputReceiver;
    private readonly Dictionary<Player, int> _scores;

    public PlayGameMode(IPingPongGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;
        _player1InputReceiver = new EntityInputReceiver(_player1Paddle);
        _player2InputReceiver = new EntityInputReceiver(_player2Paddle);
        _scores = new Dictionary<Player, int>();
    }

    public int Player1Score => GetPlayerScore(Player.Player1);
    public int Player2Score => GetPlayerScore(Player.Player2);

    private int GetPlayerScore(Player player)
    {
        return _scores.TryGetValue(player, out var score) ? score : 0;
    }

    void IGameMode.Activate()
    {
        _world.RemoveAllEntities();
        _world.RemoveAllSystems();
        _world.UnregisterAllQueries();

        ResetScores();

        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);

        _ball = _world.SpawnBall(new Vector2(400, 300), new Vector2(1, 0), Color.Red);

        _player1Paddle = _world.SpawnPaddle(Player.Player1, new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(Player.Player2, new Vector2(750, 300), Color.Green);

        _world.SpawnGoal(Player.Player1, new Vector2(0, 300), new Vector2(10, 300));
        _world.SpawnGoal(Player.Player2, new Vector2(800, 300), new Vector2(10, 300));

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

    private void ResetScores()
    {
        _scores[Player.Player1] = 0;
        _scores[Player.Player2] = 0;
    }

    private void SetupSystems()
    {
        _world
            .System<Transform2d, Paddle>()
            .ForEach((Entity entity, ref Transform2d transform, ref Paddle paddle) =>
            {
                if (!_world.Events.TryGetEventStream<InputEvent>(entity.Id, out var inputBuffer))
                {
                    return;
                }

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

                transform.Position.Y += paddle.Speed * _engine.DeltaTime;
            });

        _world
            .System<Transform2d, Ball>()
            .ForEach((Entity entity, ref Transform2d transform, ref Ball ball) =>
            {
                transform.Position += ball.Direction * ball.Speed * _engine.DeltaTime;
            });

        _world.AddSystem(new CollisionSystem());

        _world
            .System<Transform2d, Paddle, CollisionBody>()
            .ForEach((Entity entity, ref Transform2d transform, ref Paddle paddle, ref CollisionBody body) =>
            {
                // Dont let paddles go past the walls
                var collisions = _world.Events.GetEventStream<CollisionWithEvent>(entity.Id);

                foreach (var ev in collisions.AsSpan())
                {
                    var wallEntity = _world.GetEntity(ev.EntityId);

                    if (!wallEntity.Has<Wall>())
                    {
                        continue;
                    }

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
            .ForEach((Entity entity, ref Transform2d transform, ref Ball ball, ref CollisionBody body) =>
            {
                var collisionStarted = _world.Events.GetEventStream<CollisionStartedWithEvent>(entity.Id);

                foreach (var ev in collisionStarted.AsSpan())
                {
                    var other = _world.GetEntity(ev.EntityId);

                    if (other.Has<Paddle>())
                    {
                        // Bounce off paddle, using the paddle's position to influence direction
                        ref var paddleTransform = ref other.GetRef<Transform2d>();
                        float relativeIntersectY = paddleTransform.Position.Y - transform.Position.Y;
                        float normalizedRelativeIntersectionY = relativeIntersectY / (body.HalfExtents.Y + other.GetRef<CollisionBody>().HalfExtents.Y);
                        float bounceAngle = normalizedRelativeIntersectionY * (5 * (float)(Math.PI / 12));

                        ball.Direction.X = MathF.Cos(bounceAngle) * (ball.Direction.X < 0 ? 1 : -1);
                        ball.Direction.Y = -MathF.Sin(bounceAngle);
                        ball.Direction = Vector2.Normalize(ball.Direction);
                    }
                    else if (other.Has<Wall>())
                    {
                        // Bounce off wall
                        ball.Direction.Y = -ball.Direction.Y;
                    }
                }
            });

        _world
            .System<Goal>()
            .ForEach((Entity entity, ref Goal goal) =>
            {
                var collisionStarted = _world.Events.GetEventStream<CollisionStartedWithEvent>(entity.Id);

                foreach (var ev in collisionStarted.AsSpan())
                {
                    var other = _world.GetEntity(ev.EntityId);
                    if (other.Has<Ball>())
                    {
                        _world.Events.GetEventStream<PlayerGoalHit>().AddEvent(new PlayerGoalHit(goal.PlayerNumber));
                    }
                }
            });

        _world.AddSystem(this);
    }


    void IGameMode.Deactivate()
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

    void IWorldSystem.Execute(ref WorldSystemContext data)
    {
        var world = data.World;
        var goals = world.Events.GetEventStream<PlayerGoalHit>();
        foreach (var goal in goals.AsSpan())
        {
            // Determine which player scored
            var playerWhoScored = goal.PlayerNumber == Player.Player1 ? Player.Player2 : Player.Player1;

            int currentScore = _scores[playerWhoScored];
            int newScore = currentScore + 1;
            _scores[playerWhoScored] = newScore;

            var playerWins = newScore >= MaxScore;

            Console.WriteLine($"Player {playerWhoScored} scored a goal!");
            Console.WriteLine($"Scores: Player 1 = {_scores[Player.Player1]}, Player 2 = {_scores[Player.Player2]}");

            // Reset ball position
            ref var ballTransform = ref _ball.GetRef<Transform2d>();
            ballTransform.Position = new Vector2(400, 300);

            // Reset ball direction
            ref var ball = ref _ball.GetRef<Ball>();

            var newDirection = new Vector2(playerWhoScored == Player.Player2 || playerWins ? 1 : -1, 0);
            ball.Direction = Vector2.Normalize(newDirection);

            if (playerWins)
            {
                Console.WriteLine("***********************************************");
                Console.WriteLine($"Player {playerWhoScored} wins the game!");
                Console.WriteLine("***********************************************");
                ResetScores();
                continue;
            }
        }
    }
}
