using Engine.Renderers;
using Engine.WorldManagement;

namespace SneakySnake;

internal static class Builders
{
    public static IWorld CreateWorld()
    {
        IRenderer[] renderers = [new BasicShapeRenderer(), new TextRenderer()];
        IWorldSystem[] systems = [new SnakeControlSystem()];
        IWorldRenderer worldRenderer = new ExampleWorldRenderer();

        return new World(
            systems,
            renderers,
            worldRenderer);
    }
}