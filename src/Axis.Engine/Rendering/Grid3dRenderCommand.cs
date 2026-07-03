using Axis.Core.Collections;
using Raylib_cs;

namespace Axis.Engine.Rendering;

using static CommandQueue<RenderContext, RenderCommand>;

public static class DebugGrid3dRenderCommand
{
    public static void AddDebugGrid(
        this RenderCommandQueue queue,
        int slices,
        float spacing,
        int zOrder)
    {
        var payload = new Payload(slices, spacing);

        queue.Write(ref payload, Apply, zOrder);
    }

    private static readonly CommandAction Apply = (ref RenderContext context, CommandPayload payload) =>
    {
        ref Payload value = ref payload.GetRef<Payload>();

        Raylib.DrawGrid(value.Slices, value.Spacing);
    };


    internal struct Payload(int slices, float spacing)
    {
        public int Slices = slices;
        public float Spacing = spacing;
    }
}
