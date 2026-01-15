using System.Diagnostics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Rendering;
using PingPong.GameModes;
namespace PingPong;

internal enum PingPongGameState
{
    Initialisation,
    StartMenu,
    Playing,
}

internal interface IGameMode
{
    void Activate();
    void Deactivate();
}


interface IPingPongGame
{
    IGameEngine Engine { get; }
    IWorld World { get; }

    void SetCamera(Entity camera);

    void StartGame();
    void QuitGame();
}

internal sealed class PingPongGame : IPingPongGame, IGameInstance
{
    private IGameEngine? _engine;
    private readonly WorldRenderer _worldRenderer;
    private readonly PingPongUiRenderer _uiRenderer;
    private readonly IWorld _world;
    private IGameMode? _gameMode;
    private PingPongGameState _state;

    public PingPongGame()
    {
        _engine = null;
        _gameMode = null;
        _state = PingPongGameState.Initialisation;

        _worldRenderer = WorldRenderer.Create();
        _uiRenderer = new PingPongUiRenderer();
        _world = Axis.ECS.World.Create();
    }

    public IGameEngine Engine => _engine!;
    public IWorld World => _world;

    public void SetCamera(Entity camera)
    {
        _worldRenderer.SetCamera(camera);
    }

    public void StartGame()
    {
        Debug.Assert(_engine != null);

        var gameMode = new PlayGameMode(this);

        SwitchGameMode(PingPongGameState.Playing, gameMode);
    }

    public void QuitGame()
    {
        Debug.Assert(_engine != null);

        SwitchGameMode(PingPongGameState.StartMenu, new StartMenuGameMode(this));
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

        _engine.SetWorld(_world);

        _engine.SetViewports([
            Viewport.Fullscreen(_worldRenderer),
            Viewport.Fullscreen(_uiRenderer)
        ]);

        var gameMode = new StartMenuGameMode(this);

        SwitchGameMode(PingPongGameState.StartMenu, gameMode);
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
