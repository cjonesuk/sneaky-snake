using System.Diagnostics;
using System.Numerics;
using Axis.Core.Text;
using Axis.Engine;
using Axis.Engine.Rendering;
using Raylib_cs;
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

}


internal sealed class PingPongUiRenderer : IRenderer
{
    public void GenerateRenderCommands(
        ref RenderContext context,
        RenderCommandQueue renderCommands,
        out RenderMode renderMode)
    {
        renderMode = RenderMode.Create2d(
            Vector2.Zero,
            1.0f,
            0.0f);

        Span<byte> buffer = stackalloc byte[256];
        Utf8StringBuilder sb = new Utf8StringBuilder(buffer);
        sb.Write("Ping Pong Game");
        var textIndex = sb.CommitTo(context.FrameResources.TextBuffer, addNull: true);

        renderCommands.AddText(textIndex, new Vector2(10, 10), 20, Color.Red, 1);
    }
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
