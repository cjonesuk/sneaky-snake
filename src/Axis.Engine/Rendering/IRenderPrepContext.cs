using Axis.Core.Collections;
using Axis.Engine.Resources;

namespace Axis.Engine.Rendering;

/// <summary>
/// Context for preparing render commands and setting the render mode.
/// </summary>
public interface IRenderPrepContext
{
    Resolution Resolution { get; }
    FrameResources FrameResources { get; }
    RenderCommandQueue RenderCommands { get; }
    RenderMode RenderMode { get; }

    void SetRenderMode(RenderMode renderMode);
}
