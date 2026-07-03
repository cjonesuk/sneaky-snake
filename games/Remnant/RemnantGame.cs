using System.Diagnostics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Rendering;
using Axis.Engine.UI;
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
    UiContext Ui { get; }
    IGameMode? CurrentGameMode { get; }

    void SetCamera(Entity camera);
}

internal sealed class RemnantGame : IRemnantGame, IGameInstance
{
    private IGameEngine? _engine;
    private readonly World3dRenderer _worldRenderer;
    private readonly RemnantUiRenderer _fpsRenderer;
    private readonly UiContext _ui;
    private readonly UiRenderer _uiRenderer;
    private readonly IWorld _world;
    private IGameMode? _gameMode;

    public RemnantGame()
    {
        _engine = null;
        _gameMode = null;
        _world = Axis.ECS.World.Create();
        _worldRenderer = World3dRenderer.Create(_world, gridSlices: 100, gridSpacing: 1f, drawGrid: true);
        _fpsRenderer = new RemnantUiRenderer(this);
        _ui = new UiContext();
        _uiRenderer = new UiRenderer(_ui);
    }

    public IGameEngine Engine => _engine!;
    public IWorld World => _world;
    public UiContext Ui => _ui;
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
            Viewport.Fullscreen(_fpsRenderer),
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
