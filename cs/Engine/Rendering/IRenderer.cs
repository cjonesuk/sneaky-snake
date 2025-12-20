using Engine;

public interface IRenderer : IRenderQueue
{
    Type CommandType { get; }

    void Render();
}
