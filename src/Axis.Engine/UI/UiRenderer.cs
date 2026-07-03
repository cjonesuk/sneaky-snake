using Axis.Engine.Rendering;

namespace Axis.Engine.UI;

/// <summary>Screen-space renderer that flushes an <see cref="UiContext"/>'s recorded draw ops.</summary>
public sealed class UiRenderer : IRenderer
{
    private readonly UiContext _uiContext;

    public UiRenderer(UiContext uiContext)
    {
        _uiContext = uiContext;
    }

    public void Apply(IRenderPrepContext context)
    {
        context.SetRenderMode(RenderMode.CreateScreenSpace());
        _uiContext.Flush(context);
    }
}
