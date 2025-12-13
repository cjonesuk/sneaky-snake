using System.Diagnostics;
using Engine;
using Raylib_cs;

namespace SneakySnake;

public abstract class RenderSystem : ISystem
{
    protected IGameEngine? _engine;
    protected readonly List<ILayer> _layers = new();
    protected Color _clearColor;

    protected RenderSystem(Color clearColor)
    {
        _clearColor = clearColor;
    }

    public virtual void Attached(IGameEngine engine)
    {
        Debug.Assert(_engine == null, "System is already attached to an engine.");

        _engine = engine;

        var layers = CreateLayers(engine);
        _layers.AddRange(layers);
    }

    protected abstract IEnumerable<ILayer> CreateLayers(IGameEngine engine);

    public virtual void Detached()
    {
        _engine = null!;
        _layers.Clear();
    }

    public virtual void Update(float deltaTime)
    {
        Debug.Assert(_engine != null, "Engine should be attached before updating the system.");

        Raylib.ClearBackground(_clearColor);

        foreach (var layer in _layers)
        {
            layer.Render();
        }
    }
}
