using Engine;
using Engine.Rendering;

namespace SneakySnake;

internal static class Builders
{
    public static IWorld CreateWorld()
    {
        IRenderer[] renderers = [new BasicShapeRenderer(), new TextRenderer()];
        ISystem[] systems = [];

        return new World(new EntityComponentManager(), systems, renderers, new ExampleRenderQueueWorldTransformer());
    }
}