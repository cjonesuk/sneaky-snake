using Axis.Engine.Rendering;

namespace Axis.Engine.UI;

/// <summary>Screen-space renderer that flushes an <see cref="UiContext"/>'s recorded draw ops.</summary>
public sealed class UiRenderer : IRenderer
{
    private readonly UiContext _context;

    public UiRenderer(UiContext context)
    {
        _context = context;
    }

    public void GenerateRenderCommands(ref RenderContext context, out RenderMode renderMode)
    {
        renderMode = RenderMode.CreateScreenSpace();
        _context.Flush(ref context);
    }
}
