using Raylib_cs;

namespace Engine.Components;

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
