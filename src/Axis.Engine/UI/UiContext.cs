using System.Numerics;
using System.Runtime.CompilerServices;
using Axis.Core.Text;
using Axis.Engine.Input;
using Axis.Engine.Rendering;
using Raylib_cs;

namespace Axis.Engine.UI;

/// <summary>Immediate-mode UI context. Widget calls record draw ops; <see cref="UiRenderer"/> flushes them.</summary>
public sealed class UiContext
{
    private static readonly Color PanelBackground = new(20, 20, 28, 220);
    private static readonly Color ButtonNormal = new(60, 60, 80, 235);
    private static readonly Color ButtonHover = new(90, 95, 130, 245);
    private static readonly Color ButtonPressed = new(40, 50, 90, 255);
    private static readonly Color TextColor = Color.White;
    private const int DefaultFontSize = 20;

    private readonly List<DrawOp> _drawList = new();
    private readonly Stack<int> _panelStack = new();
    private MouseState? _mouse;
    private int _hotId;
    private int _activeId;

    /// <summary>Begin a new UI frame. Call once per frame before any widget calls.</summary>
    public void BeginFrame(MouseState mouse)
    {
        _mouse = mouse;
        _drawList.Clear();
        _panelStack.Clear();
        _hotId = 0;
    }

    /// <summary>Background panel claiming its rect for capture and ID scoping for nested widgets.</summary>
    public void BeginPanel(string id, Rectangle rect, [CallerLineNumber] int line = 0)
    {
        int panelId = WidgetId(id, line);
        _panelStack.Push(panelId);

        if (RectContains(rect, Mouse.Position))
        {
            Mouse.Capture();
        }

        _drawList.Add(new DrawOp
        {
            Kind = DrawOpKind.Rectangle,
            Color = PanelBackground,
            Rectangle = rect,
        });
    }

    public void EndPanel()
    {
        if (_panelStack.Count == 0)
        {
            throw new InvalidOperationException("EndPanel called without matching BeginPanel.");
        }
        _panelStack.Pop();
    }

    /// <summary>Render text. Decorative only — never captures the mouse.</summary>
    public void Label(string text, Vector2 position, int fontSize = DefaultFontSize)
    {
        Label(text, position, TextColor, fontSize);
    }

    public void Label(string text, Vector2 position, Color color, int fontSize = DefaultFontSize)
    {
        _drawList.Add(new DrawOp
        {
            Kind = DrawOpKind.Text,
            Color = color,
            Text = text,
            TextPosition = position,
            FontSize = fontSize,
            TextAlignment = TextAlignment.Left,
            TextVerticalAlignment = TextVerticalAlignment.Top,
        });
    }

    /// <summary>Clickable button. Returns true only on the frame the click completes over the button.</summary>
    public bool Button(string label, Rectangle rect, [CallerLineNumber] int line = 0)
    {
        int id = WidgetId(label, line);
        var mouse = Mouse;

        bool hovered = RectContains(rect, mouse.Position);
        if (hovered)
        {
            _hotId = id;
            mouse.Capture();
        }

        if (hovered && mouse.WasPressed(MouseButton.Left))
        {
            _activeId = id;
        }

        bool clicked = false;
        if (_activeId == id && mouse.WasReleased(MouseButton.Left))
        {
            if (hovered)
            {
                clicked = true;
            }
            _activeId = 0;
        }

        Color background;
        if (_activeId == id && hovered)
        {
            background = ButtonPressed;
        }
        else if (hovered)
        {
            background = ButtonHover;
        }
        else
        {
            background = ButtonNormal;
        }

        _drawList.Add(new DrawOp
        {
            Kind = DrawOpKind.Rectangle,
            Color = background,
            Rectangle = rect,
        });

        _drawList.Add(new DrawOp
        {
            Kind = DrawOpKind.Text,
            Color = TextColor,
            Text = label,
            TextPosition = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f),
            FontSize = DefaultFontSize,
            TextAlignment = TextAlignment.Center,
            TextVerticalAlignment = TextVerticalAlignment.Center,
        });

        return clicked;
    }

    /// <summary>Replay recorded ops into the render command queue. Called by <see cref="UiRenderer"/>.</summary>
    public void Flush(ref RenderContext context)
    {
        int zOrder = 0;
        var queue = context.RenderCommands;
        var textBuffer = context.FrameResources.TextBuffer;
        var origin = Vector2.Zero;
        Span<byte> textBytes = stackalloc byte[256];

        foreach (var op in _drawList)
        {
            if (op.Kind == DrawOpKind.Rectangle)
            {
                var rect = op.Rectangle;
                queue.AddRectangle(ref rect, ref origin, 0f, op.Color, zOrder++);
                continue;
            }

            var sb = new Utf8StringBuilder(textBytes);
            sb.Write(op.Text!);
            var slice = sb.CommitTo(textBuffer, addNull: true);

            queue.AddText(
                slice,
                op.TextPosition,
                op.TextAlignment,
                op.TextVerticalAlignment,
                op.FontSize,
                op.Color,
                zOrder++);
        }
    }

    private MouseState Mouse
    {
        get
        {
            if (_mouse is null)
            {
                throw new InvalidOperationException("BeginFrame must be called before any widget calls.");
            }
            return _mouse;
        }
    }

    private int WidgetId(string label, int callerLine)
    {
        int parentId = _panelStack.Count > 0 ? _panelStack.Peek() : 0;
        return HashCode.Combine(parentId, label, callerLine);
    }

    private static bool RectContains(Rectangle rect, Vector2 point)
    {
        return point.X >= rect.X
            && point.Y >= rect.Y
            && point.X < rect.X + rect.Width
            && point.Y < rect.Y + rect.Height;
    }
}
