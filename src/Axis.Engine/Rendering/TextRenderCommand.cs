using System.Numerics;
using Axis.Core.Collections;
using Axis.Core.Memory;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;



public static class TextRenderCommand
{
    public static void AddText(
        this RenderCommandQueue queue,
        NativeSlice text,
        Vector2 position,
        int fontSize,
        Color color,
        int zOrder)
    {
        var payload = new Payload(text, position, fontSize, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private unsafe static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        sbyte* textPtr = context.FrameResources.TextBuffer.GetPtr<sbyte>(value.Text.Offset);

        Raylib.DrawText(textPtr, (int)value.Position.X, (int)value.Position.Y, value.FontSize, value.Color);
    };

    internal struct Payload(NativeSlice text, Vector2 position, int fontSize, Color color)
    {
        public NativeSlice Text = text;
        public Vector2 Position = position;
        public int FontSize = fontSize;
        public Color Color = color;
    }
}