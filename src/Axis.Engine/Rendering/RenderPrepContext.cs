using Axis.Core.Collections;
using Axis.Engine.Resources;

namespace Axis.Engine.Rendering;

public sealed class RenderPrepContext : IRenderPrepContext
{
    public Resolution Resolution { get; }
    public FrameResources FrameResources { get; }
    public RenderCommandQueue RenderCommands { get; }
    public RenderMode RenderMode { get; private set; }

    private RenderPrepContext(Resolution resolution, FrameResources frameResources, RenderCommandQueue renderCommands)
    {
        Resolution = resolution;
        FrameResources = frameResources;
        RenderCommands = renderCommands;
        RenderMode = RenderMode.None;
    }

    public static RenderPrepContext Create(Resolution resolution, FrameResources frameResources, RenderCommandQueue renderCommands)
    {
        return new RenderPrepContext(resolution, frameResources, renderCommands);
    }

    public void SetRenderMode(RenderMode renderMode)
    {
        RenderMode = renderMode;
    }
}
