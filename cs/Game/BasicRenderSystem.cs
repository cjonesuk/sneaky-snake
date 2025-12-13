using Engine;
using Raylib_cs;

namespace SneakySnake;

public class BasicRenderSystem : RenderSystem
{
    public BasicRenderSystem(Color clearColor) : base(clearColor)
    {
    }

    protected override IEnumerable<ILayer> CreateLayers(IGameEngine engine)
    {
        yield return new EntityLayer(engine);
        yield return new UiLayer(engine);
    }
}
