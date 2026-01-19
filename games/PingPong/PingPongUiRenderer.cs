using System.Numerics;
using Axis.Core.Text;
using Axis.Engine.Rendering;
using PingPong.PlayGame;
using PingPong.StartMenu;
using Raylib_cs;

namespace PingPong;

internal sealed class PingPongUiRenderer : IRenderer
{
    private readonly IPingPongGame _game;

    public PingPongUiRenderer(IPingPongGame game)
    {
        _game = game;
    }

    public void GenerateRenderCommands(
        ref RenderContext context,
        out RenderMode renderMode)
    {
        renderMode = RenderMode.CreateScreenSpace();

        var gameMode = _game.CurrentGameMode;
        if (gameMode is null)
        {
            return;
        }

        if (gameMode is StartMenuGameMode)
        {
            Span<byte> buffer = stackalloc byte[256];
            Utf8StringBuilder sb = new Utf8StringBuilder(buffer);
            sb.Write("Ping Pong Game");
            var textIndex = sb.CommitTo(context.FrameResources.TextBuffer, addNull: true);

            context.RenderCommands.AddText(textIndex, new Vector2(0, 0), 20, Color.Red, 1);
        }
        else if (gameMode is PlayGameMode playGameMode)
        {
            Span<byte> buffer = stackalloc byte[256];
            Utf8StringBuilder sb = new Utf8StringBuilder(buffer);
            sb.Write($"Player 1: {playGameMode.Player1Score}   Player 2: {playGameMode.Player2Score}");
            var textIndex = sb.CommitTo(context.FrameResources.TextBuffer, addNull: true);

            context.RenderCommands.AddText(textIndex, new Vector2(0, 0), 20, Color.White, 1);
        }
    }
}
