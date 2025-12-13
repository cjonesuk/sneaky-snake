using Raylib_cs;

namespace SneakySnake;

internal class BackgroundLayer : ILayer
{
    private readonly IEngine _engine;
    private readonly Color _backgroundColor;

    public BackgroundLayer(IEngine engine, Color backgroundColor)
    {
        _engine = engine;
        _backgroundColor = backgroundColor;
    }

    public void Render()
    {
        Raylib.ClearBackground(_backgroundColor);
    }
}
