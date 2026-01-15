using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Axis.Engine.Rendering;
using Raylib_cs;

namespace PingPong.GameModes;

internal sealed class PlayGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private Entity _camera;
    private Entity _ball;
    private Entity _player1Paddle;
    private Entity _player2Paddle;


    public PlayGameMode(IPingPongGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;
    }

    public void Activate()
    {
        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);
        _ball = _world.SpawnBall(new Vector2(400, 300), Color.Red);
        _player1Paddle = _world.SpawnPaddle(new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(new Vector2(750, 300), Color.Green);

        _engine.Devices.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Q, PlayGameActions.QuitGame)
                ]
            )
        ]);

        _game.SetCamera(_camera);

        Console.WriteLine("Start Menu Activated");
    }

    public void Deactivate()
    {
        _engine.Devices.KeyboardAndMouse.ClearContext();
        _engine.ClearWorld();

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
