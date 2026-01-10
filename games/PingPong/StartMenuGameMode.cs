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
    private readonly IWorld _world;
    private readonly Entity _camera;
    private readonly Entity _ball;


    public StartMenuGameMode(IPingPongGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = World.Create();
        _camera = _world.SpawnCamera2d(new Vector2(400, 300), 1.0f);
        _ball = _world.SpawnBall(new Vector2(400, 300), Color.Red);
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

        _engine.SetViewports(
        [
            Viewport.Fullscreen(_camera)
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
