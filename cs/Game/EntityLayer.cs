using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class EntityLayer : ILayer
{
    private readonly IGameEngine _engine;

    public EntityLayer(IGameEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        Raylib.DrawRectangle(50, 50, 100, 100, Color.Yellow);
    }
}
