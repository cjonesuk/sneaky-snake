using System.Numerics;
using Axis.Core.Text;
using Axis.Engine.Rendering;
using Raylib_cs;

namespace Remnant;

internal sealed class RemnantUiRenderer : IRenderer
{
    private readonly IRemnantGame _game;

    public RemnantUiRenderer(IRemnantGame game)
    {
        _game = game;
    }

    public void GenerateRenderCommands(
        ref RenderContext context,
        out RenderMode renderMode)
    {
        renderMode = RenderMode.CreateScreenSpace();

        Span<byte> buffer = stackalloc byte[64];
        Utf8StringBuilder sb = new Utf8StringBuilder(buffer);
        sb.Write($"FPS: {Raylib.GetFPS()}");
        var textIndex = sb.CommitTo(context.FrameResources.TextBuffer, addNull: true);

        context.RenderCommands.AddText(textIndex, new Vector2(10, 10), TextAlignment.Left, TextVerticalAlignment.Top, 20, Color.White, 1);
    }
}
