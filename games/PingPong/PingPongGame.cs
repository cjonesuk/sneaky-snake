using System.Diagnostics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Raylib_cs;
namespace PingPong;

internal enum PingPongGameState
{
    Initialisation,
    StartMenu,
    Playing,
}

interface IPingPongGame
{

}

internal sealed class PingPongGame : IPingPongGame, IGameInstance
{
    private IGameEngine? _engine;
    private IGameMode? _gameMode;
    private PingPongGameState _state;

    public PingPongGame()
    {
        _engine = null;
        _gameMode = null;
        _state = PingPongGameState.Initialisation;
    }

    public void OnEngineStart(IGameEngine engine)
    {
        Debug.Assert(_engine == null);

        _engine = engine;

        Initialise();

        Console.WriteLine("PingPong Game Started");
    }

    public void OnEngineStop(IGameEngine engine)
    {
        Debug.Assert(_engine == engine);

        _engine = null;
        Console.WriteLine("PingPong Game Stopped");
    }

    private void Initialise()
    {
        Debug.Assert(_engine != null);

        _gameMode = new StartMenuGameMode(this, _engine);

        SwitchGameMode(PingPongGameState.StartMenu, _gameMode);
    }

    private void SwitchGameMode(PingPongGameState state, IGameMode gameMode)
    {
        Debug.Assert(_engine != null);

        _gameMode?.Deactivate();
        _gameMode = gameMode;
        _gameMode.Activate();
        _state = state;
    }
}

internal interface IGameMode
{
    void Activate();
    void Deactivate();
}

internal sealed class StartMenuGameMode : IGameMode, IInputReceiver
{
    private readonly IPingPongGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;


    public StartMenuGameMode(IPingPongGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
        _world = World.Create();
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
