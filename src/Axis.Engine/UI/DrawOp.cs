using System.Numerics;
using Axis.Engine.Rendering;
using Raylib_cs;

namespace Axis.Engine.UI;

internal enum DrawOpKind
{
    Rectangle,
    Text,
}

/// <summary>One IMGUI draw command, recorded by widget calls and replayed by <see cref="UiRenderer"/>.</summary>
internal struct DrawOp
{
    public DrawOpKind Kind;
    public Color Color;
    public Rectangle Rectangle;
    public Vector2 TextPosition;
    public string? Text;
    public int FontSize;
    public TextAlignment TextAlignment;
    public TextVerticalAlignment TextVerticalAlignment;
}
