using System.Numerics;
using Axis.Core.Collections;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

internal static class CircleRenderCommand
{
    public static void AddCircle(
        this RenderCommandQueue queue,
        ref Vector2 center,
         float radius,
         Color color,
         int zOrder)
    {
        var payload = new Payload(center, radius, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        Raylib.DrawCircleV(value.Center, value.Radius, value.Color);
    };

    internal struct Payload(Vector2 center, float radius, Color color)
    {
        public Vector2 Center = center;
        public float Radius = radius;
        public Color Color = color;
    }
}
