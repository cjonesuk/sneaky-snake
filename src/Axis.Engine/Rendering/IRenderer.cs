namespace Axis.Engine.Rendering;

public interface IRenderer
{
    void GenerateRenderCommands(RenderCommandQueue renderCommands, out RenderMode renderMode);
}

