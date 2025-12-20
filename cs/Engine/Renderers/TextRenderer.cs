using Raylib_cs;
using SneakySnake.Engine.Rendering;

namespace Engine.Renderers;

public sealed class TextRenderer : RenderQueueRenderer<TextRenderCommand>
{
    public override void RenderCommands()
    {
        var font = Raylib.GetFontDefault();

        for (int index = 0; index < _commands.Count; ++index)
        {
            var cmd = _commands[index];
            Raylib.DrawTextEx(font, cmd.Text, cmd.Position, cmd.FontSize, cmd.Spacing, cmd.Color);
        }
    }
}