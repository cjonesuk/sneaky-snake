using System.Numerics;
using Raylib_cs;

namespace Engine.Renderers;

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
