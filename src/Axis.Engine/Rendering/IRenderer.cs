namespace Axis.Engine.Rendering;

public interface IRenderer
{
    void GenerateRenderCommands(
        ref RenderContext context,
        RenderCommandQueue renderCommands,
        out RenderMode renderMode);
}

