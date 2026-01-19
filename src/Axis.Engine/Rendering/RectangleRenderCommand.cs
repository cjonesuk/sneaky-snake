using System.Numerics;
using Axis.Core.Collections;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public static class RectangleRenderCommand
{
    public static void AddRectangle(
        this RenderCommandQueue queue,
        ref Rectangle rect,
        ref Vector2 origin,
        float rotation,
        Color color,
        int zOrder)
    {
        var payload = new Payload(rect, origin, rotation, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        Raylib.DrawRectanglePro(value.Rec, value.Origin, value.Rotation, value.Color);
    };

    internal struct Payload(Rectangle rec, Vector2 origin, float rotation, Color color)
    {
        public Rectangle Rec = rec;
        public Vector2 Origin = origin;
        public float Rotation = rotation;
        public Color Color = color;
    }
}
