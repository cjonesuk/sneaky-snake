using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Raylib_cs;

namespace PingPong.GameModes;

internal sealed class StartMenuGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private Entity _camera;


    public StartMenuGameMode(IPingPongGame game)
    {
        _game = game;
        _engine = game.Engine;
    }

    public void Activate()
    {
        _camera = _game.World.SpawnCamera2d(new Vector2(400, 300), 1.0f);

        _engine.Devices.KeyboardAndMouse.BindContext([
            new KeyboardInputContext(
                this,
                keyDown: [],
                keyPressed: [
                    new KeyboardInputMapping(KeyboardKey.Enter, StartMenuActions.StartGame)
                ]
            )
        ]);

        _game.SetCamera(_camera);

        Console.WriteLine("Start Menu Activated");
    }

    public void Deactivate()
    {
        _engine.Devices.KeyboardAndMouse.ClearContext();

        Console.WriteLine("Start Menu Deactivated");
    }

    void IInputReceiver.ReceiveInput(InputEvent inputEvent)
    {
        if (inputEvent.Id == StartMenuActions.StartGame)
        {
            Console.WriteLine("Start Game action received!");
            _game.StartGame();
        }
    }
}
