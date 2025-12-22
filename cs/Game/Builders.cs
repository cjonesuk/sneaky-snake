using Engine;
using Engine.Renderers;
using Engine.Rendering;

namespace SneakySnake;

internal static class Builders
{
    public static IWorld CreateWorld()
    {
        IRenderer[] renderers = [new BasicShapeRenderer(), new TextRenderer()];
        IWorldSystem[] systems = [new SnakeControlSystem()];

        return new World(
            new EntityComponentManager(),
            systems,
            renderers,
            new ExampleWorldRenderer());
    }
}