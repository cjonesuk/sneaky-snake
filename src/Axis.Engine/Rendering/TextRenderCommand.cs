using System.Numerics;
using Axis.Core.Collections;
using Axis.Core.Memory;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public enum TextAlignment
{
    Left,
    Center,
    Right
}

public enum TextVerticalAlignment
{
    Top,
    Center,
    Bottom
}

public static class TextRenderCommand
{
    public static void AddText(
        this RenderCommandQueue queue,
        NativeSlice text,
        Vector2 position,
        TextAlignment alignment,
        TextVerticalAlignment verticalAlignment,
        int fontSize,
        Color color,
        int zOrder)
    {
        var payload = new Payload(text, position, alignment, verticalAlignment, fontSize, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private unsafe static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        sbyte* textPtr = context.FrameResources.TextBuffer.GetPtr<sbyte>(value.Text.Offset);

        var font = Raylib.GetFontDefault();
        var textSize = Raylib.MeasureTextEx(font, textPtr, value.FontSize, 1);

        float x = value.Alignment switch
        {
            TextAlignment.Left => value.Position.X,
            TextAlignment.Center =>
                value.Position.X - (textSize.X / 2),
            TextAlignment.Right =>
                value.Position.X - textSize.X,
            _ => value.Position.X
        };

        float y = value.VerticalAlignment switch
        {
            TextVerticalAlignment.Top => value.Position.Y,
            TextVerticalAlignment.Center =>
                value.Position.Y - (textSize.Y / 2),
            TextVerticalAlignment.Bottom =>
                value.Position.Y - textSize.Y,
            _ => value.Position.Y
        };

        Raylib.DrawText(textPtr, (int)x, (int)y, value.FontSize, value.Color);
    };

    internal struct Payload(NativeSlice text, Vector2 position, TextAlignment alignment, TextVerticalAlignment verticalAlignment, int fontSize, Color color)
    {
        public NativeSlice Text = text;
        public Vector2 Position = position;
        public TextAlignment Alignment = alignment;
        public TextVerticalAlignment VerticalAlignment = verticalAlignment;
        public int FontSize = fontSize;
        public Color Color = color;
    }
}