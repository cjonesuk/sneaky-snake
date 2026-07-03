using Axis.Core.Collections;
using Axis.Engine.Resources;

namespace Axis.Engine.Rendering;

/// <summary>
/// Content for when applying render commands.
/// </summary>
/// <param name="resolution"></param>
/// <param name="frameResources"></param>
public readonly struct RenderContext(Resolution resolution, FrameResources frameResources)
{
    public readonly Resolution Resolution = resolution;
    public readonly FrameResources FrameResources = frameResources;
}
