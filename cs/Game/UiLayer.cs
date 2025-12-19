using System.Numerics;
using Raylib_cs;
using SneakySnake.Engine.Rendering;

namespace SneakySnake;

public enum TextAlignment
{
    Left,
    Center
}

public struct Text2d
{
    public string Text;
    public float FontSize;
    public Color Color;
    public TextAlignment Alignment;

    public Text2d(string text, float fontSize, Color color, TextAlignment alignment)
    {
        Text = text;
        FontSize = fontSize;
        Color = color;
        Alignment = alignment;
    }
}


public readonly struct TextRenderCommand
{
    public readonly Vector2 Position;
    public readonly float FontSize;
    public readonly float Spacing;
    public readonly Color Color;
    public readonly string Text;

    public TextRenderCommand(Vector2 position, float fontSize, float spacing, Color color, string text)
    {
        Position = position;
        FontSize = fontSize;
        Spacing = spacing;
        Color = color;
        Text = text;
    }
}

public sealed class TextRenderer : RenderQueue<TextRenderCommand>
{
    public override void Render()
    {
        var font = Raylib.GetFontDefault();

        for (int index = 0; index < _commands.Count; ++index)
        {
            var cmd = _commands[index];
            Raylib.DrawTextEx(font, cmd.Text, cmd.Position, cmd.FontSize, cmd.Spacing, cmd.Color);
        }
    }
}