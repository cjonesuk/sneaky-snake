
using System.Numerics;

public interface IRenderPass
{
    void SetCamera(Camera2dRenderView cameraView);
    IRenderQueue<TCommand> GetQueue<TCommand>() where TCommand : struct;
}

public readonly struct Camera2dRenderView
{
    public readonly Vector2 Target;
    public readonly float Zoom;
    public readonly float Rotation;

    public Camera2dRenderView(Vector2 target, float zoom, float rotation)
    {
        Target = target;
        Zoom = zoom;
        Rotation = rotation;
    }

}

/// <summary>
/// Represents a rendering pass that contains a renderer for each type of render command.
/// </summary>
public sealed class RenderPass : IRenderPass
{
    private readonly IRenderer[] _renderers;
    private readonly Dictionary<Type, IRenderQueue> _renderQueueByType;
    private Camera2dRenderView _camera2dView;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderPass"/> struct.
    /// </summary>
    /// <param name="renderers">An ordered array of renderers.</param>
    public RenderPass(IRenderer[] renderers)
    {
        _renderers = renderers;
        _renderQueueByType = new Dictionary<Type, IRenderQueue>(_renderers.Length);
    }

    public void SetCamera(Camera2dRenderView cameraView)
    {
        _camera2dView = cameraView;
    }

    public Camera2dRenderView GetCamera()
    {
        return _camera2dView;
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