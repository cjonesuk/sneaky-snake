namespace Axis.Engine.Rendering;

public interface IRenderer
{
    void GenerateRenderCommands(
        ref RenderContext context,
        out RenderMode renderMode);
}

