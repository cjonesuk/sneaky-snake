using Raylib_cs;

namespace SneakySnake;

internal class EntityLayer : ILayer
{
    private readonly IEngine _engine;

    public EntityLayer(IEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        Raylib.DrawRectangle(50, 50, 100, 100, Color.Yellow);
    }
}
