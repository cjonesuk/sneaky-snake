using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class BackgroundLayer : ILayer
{
    private readonly IGameEngine _engine;
    private readonly Color _backgroundColor;

    public BackgroundLayer(IGameEngine engine, Color backgroundColor)
    {
        _engine = engine;
        _backgroundColor = backgroundColor;
    }

    public void Render()
    {
        Raylib.ClearBackground(_backgroundColor);
    }
}
