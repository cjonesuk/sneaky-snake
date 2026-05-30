using System.Numerics;
using Axis.Core.Collections;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public static class Cube3dWiresRenderCommand
{
    public static void AddCubeWires(
        this RenderCommandQueue queue,
        Vector3 position,
        Vector3 size,
        Color color,
        int zOrder)
    {
        var payload = new Payload(position, size, color);

        queue.Write(ref payload, Apply, zOrder);
    }

    private static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        Raylib.DrawCubeWires(value.Position, value.Size.X, value.Size.Y, value.Size.Z, value.Color);
    };

    internal struct Payload(Vector3 position, Vector3 size, Color color)
    {
        public Vector3 Position = position;
        public Vector3 Size = size;
        public Color Color = color;
    }
}
