using System.Numerics;
using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class UiLayer : ILayer
{
    private readonly IGameEngine _engine;
    private readonly Font _font;

    public UiLayer(IGameEngine engine)
    {
        _engine = engine;
        _font = Raylib.GetFontDefault();
    }

    public void Render()
    {
        Vector2 textSize = Raylib.MeasureTextEx(_font, "Welcome to Sneaky Snake!", 20, 1);
        Vector2 textPosition = new Vector2((_engine.Settings.ScreenWidth / 2) - (textSize.X / 2), (_engine.Settings.ScreenHeight / 2) - (textSize.Y / 2));

        Raylib.DrawTextEx(_font, "Welcome to Sneaky Snake!", textPosition, 20, 1, Color.Black);
    }
}
