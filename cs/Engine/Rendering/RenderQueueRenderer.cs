namespace SneakySnake.Engine.Rendering;

public abstract class RenderQueueRenderer<TCommand>
    : IRenderQueue<TCommand>, IRenderer
    where TCommand : struct
{
    protected readonly List<TCommand> _commands;

    public RenderQueueRenderer()
    {
        _commands = new List<TCommand>();
    }

    Type IRenderer.CommandType => typeof(TCommand);

    void IRenderQueue<TCommand>.Enqueue(TCommand command)
    {
        _commands.Add(command);
    }

    IReadOnlyList<TCommand> IRenderQueue<TCommand>.Commands => _commands;

    public void Render()
    {
        RenderCommands();
        _commands.Clear();
    }

    public abstract void RenderCommands();
}
