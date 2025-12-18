namespace SneakySnake.Engine.Rendering;

public abstract class RenderQueue<TCommand>
    : IRenderQueue<TCommand>, IRenderer
    where TCommand : struct
{
    protected readonly List<TCommand> _commands;

    public RenderQueue()
    {
        _commands = new List<TCommand>();
    }

    Type IRenderer.CommandType => typeof(TCommand);

    void IRenderQueue<TCommand>.Enqueue(TCommand command)
    {
        _commands.Add(command);
    }

    IReadOnlyList<TCommand> IRenderQueue<TCommand>.Commands => _commands;

    public abstract void Render();
}
