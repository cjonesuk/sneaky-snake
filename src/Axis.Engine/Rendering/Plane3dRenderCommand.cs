using System.Numerics;
using Axis.Core.Collections;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public static class Plane3dRenderCommand
{
    public static void AddPlane(
        this RenderCommandQueue queue,
        Vector3 position,
        Vector2 sizeXZ,
        Color color,
        int zOrder)
    {
        var payload = new Payload(position, sizeXZ, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        Raylib.DrawPlane(value.Position, value.SizeXZ, value.Color);
    };

    internal struct Payload(Vector3 position, Vector2 sizeXZ, Color color)
    {
        public Vector3 Position = position;
        public Vector2 SizeXZ = sizeXZ;
        public Color Color = color;
    }
}
