using System.Numerics;
using Axis.Core.Text;
using Axis.Engine.Rendering;
using Raylib_cs;
namespace PingPong;

internal sealed class PingPongUiRenderer : IRenderer
{
    public void GenerateRenderCommands(
        ref RenderContext context,
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

        context.RenderCommands.AddText(textIndex, new Vector2(10, 10), 20, Color.Red, 1);
    }
}
