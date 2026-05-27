using System.Diagnostics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Rendering;
using Remnant.PlayGame;

namespace Remnant;

internal interface IGameMode
{
    void Activate();
    void Deactivate();
}

internal interface IRemnantGame
{
    IGameEngine Engine { get; }
    IWorld World { get; }
    IGameMode? CurrentGameMode { get; }

    void SetCamera(Entity camera);
}

internal sealed class RemnantGame : IRemnantGame, IGameInstance
{
    private IGameEngine? _engine;
    private readonly World3dRenderer _worldRenderer;
    private readonly RemnantUiRenderer _uiRenderer;
    private readonly IWorld _world;
    private IGameMode? _gameMode;

    public RemnantGame()
    {
        _engine = null;
        _gameMode = null;
        _worldRenderer = World3dRenderer.Create(gridSlices: 100, gridSpacing: 1f, drawGrid: true);
        _uiRenderer = new RemnantUiRenderer(this);
        _world = Axis.ECS.World.Create();
    }

    public IGameEngine Engine => _engine!;
    public IWorld World => _world;
    public IGameMode? CurrentGameMode => _gameMode;

    public void SetCamera(Entity camera)
    {
        _worldRenderer.SetCamera(camera);
    }

    public void OnEngineStart(IGameEngine engine)
    {
        Debug.Assert(_engine == null);

        _engine = engine;

        _engine.SetWorld(_world);
        _engine.SetViewports([
            Viewport.Fullscreen(_worldRenderer),
            Viewport.Fullscreen(_uiRenderer),
        ]);

        SwitchGameMode(new PlayGameMode(this));

        Console.WriteLine("Remnant started");
    }

    public void OnEngineStop(IGameEngine engine)
    {
        Debug.Assert(_engine == engine);
        _engine = null;
        Console.WriteLine("Remnant stopped");
    }

    private void SwitchGameMode(IGameMode gameMode)
    {
        _gameMode?.Deactivate();
        _gameMode = gameMode;
        _gameMode.Activate();
    }
}
