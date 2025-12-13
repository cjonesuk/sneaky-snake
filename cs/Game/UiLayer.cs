using System.Numerics;
using Engine;
using Raylib_cs;

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

public struct Transform2d
{
    public float X;
    public float Y;

    public Transform2d(float x, float y)
    {
        X = x;
        Y = y;
    }
}

internal class UiLayer : ILayer
{
    private readonly IGameEngine _engine;
    private readonly Font _font;

    public UiLayer(IGameEngine engine)
    {
        _engine = engine;
        _font = Raylib.GetFontDefault();
    }

    public void Render()
    {
        var result = _engine.Entities.QueryAll<Transform2d, Text2d>();

        foreach (var (transform, text2d) in result)
        {
            Vector2 textSize = Raylib.MeasureTextEx(_font, text2d.Text, text2d.FontSize, 1);

            Vector2 textPosition = text2d.Alignment switch
            {
                TextAlignment.Left => new Vector2(
                    transform.X,
                    transform.Y
                ),
                TextAlignment.Center => new Vector2(
                    transform.X - (textSize.X / 2),
                    transform.Y - (textSize.Y / 2)
                ),
                _ => new Vector2(0, 0)
            };

            Raylib.DrawTextEx(_font, text2d.Text, textPosition, text2d.FontSize, 1, text2d.Color);
        }
    }
}
