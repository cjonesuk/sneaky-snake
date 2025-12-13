using Raylib_cs;

namespace SneakySnake;

internal class BackgroundLayer : ILayer
{
    private readonly IEngine _engine;
    private readonly Color _backgroundColor = Color.SkyBlue;

    public BackgroundLayer(IEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        Raylib.ClearBackground(_backgroundColor);
    }
}
