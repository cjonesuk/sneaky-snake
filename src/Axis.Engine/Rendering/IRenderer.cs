namespace Axis.Engine.Rendering;

public interface IRenderer : IRenderQueue
{
    Type CommandType { get; }

    void Render();
}
