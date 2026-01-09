namespace Axis.Engine.Rendering;

public interface IRenderQueue
{

}

public interface IRenderQueue<TCommand> : IRenderQueue where TCommand : struct
{
    void Enqueue(TCommand command);
    IReadOnlyList<TCommand> Commands { get; }
}