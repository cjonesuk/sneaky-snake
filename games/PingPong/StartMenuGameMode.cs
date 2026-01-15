using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Axis.Engine.Rendering;
using Raylib_cs;
namespace PingPong;

internal sealed class StartMenuGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly WorldRenderer _worldRenderer;
    private readonly PingPongUiRenderer _uiRenderer;
    private readonly IWorld _world;
    private readonly Entity _camera;
    private readonly Entity _ball;
    private readonly Entity _player1Paddle;
    private readonly Entity _player2Paddle;


    public StartMenuGameMode(IPingPongGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _worldRenderer = WorldRenderer.Create();
        _uiRenderer = new PingPongUiRenderer();
        _world = World.Create();
        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);
        _ball = _world.SpawnBall(new Vector2(400, 300), Color.Red);
        _player1Paddle = _world.SpawnPaddle(new Vector2(50, 300), Color.Blue);
        _player2Paddle = _world.SpawnPaddle(new Vector2(750, 300), Color.Green);
    }

    public void Activate()
    {
        _engine.SetWorld(_world);

        _engine.Devices.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Enter, StartMenuActions.StartGame)
                ]
            )
        ]);

        _worldRenderer.SetCamera(_camera);

        _engine.SetViewports(
        [
            Viewport.Fullscreen(_worldRenderer),
            Viewport.Fullscreen(_uiRenderer)
        ]);

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
        if (inputEvent.Id == StartMenuActions.StartGame)
        {
            Console.WriteLine("Start Game action received!");
        }
    }
}
