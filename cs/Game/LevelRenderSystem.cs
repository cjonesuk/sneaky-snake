using System.Diagnostics;
using Engine;
using Raylib_cs;

namespace SneakySnake;

public abstract class RenderSystem : ISystem
{
    protected IGameEngine? _engine;
    protected readonly List<ILayer> _layers = new();

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

        foreach (var layer in _layers)
        {
            layer.Render();
        }
    }
}

public class MenuRenderSystem : RenderSystem
{
    protected override IEnumerable<ILayer> CreateLayers(IGameEngine engine)
    {
        yield return new BackgroundLayer(engine, Color.SkyBlue);
        yield return new UiLayer(engine);
    }
}

public class LevelRenderSystem : RenderSystem
{
    protected override IEnumerable<ILayer> CreateLayers(IGameEngine engine)
    {
        yield return new BackgroundLayer(engine, Color.Lime);
        yield return new EntityLayer(engine);
        yield return new UiLayer(engine);
    }
}
