
public interface IRenderPass
{
    IRenderQueue<TCommand> GetQueue<TCommand>() where TCommand : struct;
}

/// <summary>
/// Represents a rendering pass that contains a renderer for each type of render command.
/// </summary>
public sealed class RenderPass : IRenderPass
{
    private readonly IRenderer[] _renderers;
    private readonly Dictionary<Type, IRenderQueue> _renderQueueByType;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderPass"/> struct.
    /// </summary>
    /// <param name="renderers">An ordered array of renderers.</param>
    public RenderPass(IRenderer[] renderers)
    {
        _renderers = renderers;
        _renderQueueByType = new Dictionary<Type, IRenderQueue>(_renderers.Length);
    }

    public IRenderQueue<TCommand> GetQueue<TCommand>() where TCommand : struct
    {
        Type commandType = typeof(TCommand);

        if (_renderQueueByType.TryGetValue(commandType, out var queue))
        {
            return (IRenderQueue<TCommand>)queue;
        }

        for (int index = 0; index < _renderers.Length; ++index)
        {
            var renderer = _renderers[index];
            if (renderer.CommandType == commandType)
            {
                _renderQueueByType[commandType] = renderer;
                return (IRenderQueue<TCommand>)renderer;
            }
        }

        throw new InvalidOperationException($"No renderer found for command type {typeof(TCommand)}.");
    }

    public void Render()
    {
        foreach (var renderer in _renderers)
        {
            renderer.Render();
        }
    }
}