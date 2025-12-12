using System.Numerics;
using Raylib_cs;

namespace SneakySnake;

interface IEngine
{
    Settings Settings { get; }

    void Run();
}

internal record Settings(int ScreenWidth, int ScreenHeight, string Title);

interface ILayer
{
    void Render();
}

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

internal class UiLayer : ILayer
{
    private readonly IEngine _engine;
    private readonly Font _font;

    public UiLayer(IEngine engine)
    {
        _engine = engine;
        _font = Raylib.GetFontDefault();
    }

    public void Render()
    {
        Vector2 textSize = Raylib.MeasureTextEx(_font, "Welcome to Sneaky Snake!", 20, 1);
        Vector2 textPosition = new Vector2((_engine.Settings.ScreenWidth - textSize.X) / 2, (_engine.Settings.ScreenHeight - textSize.Y) / 2);
        Raylib.DrawTextEx(_font, "Welcome to Sneaky Snake!", textPosition, 20, 1, Color.Black);
    }
}

internal class Engine : IEngine
{
    private readonly Settings _settings;

    public Engine(Settings settings)
    {
        _settings = settings;
    }

    public Settings Settings => _settings;

    public void Run()
    {
        Raylib.InitWindow(_settings.ScreenWidth, _settings.ScreenHeight, _settings.Title);

        ILayer backgroundLayer = new BackgroundLayer(this);
        ILayer entityLayer = new EntityLayer(this);
        ILayer uiLayer = new UiLayer(this);

        ILayer[] layers = { backgroundLayer, entityLayer, uiLayer };

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            foreach (var layer in layers)
            {
                layer.Render();
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Settings settings = new Settings(800, 600, "Sneaky Snake");
        IEngine engine = new Engine(settings);
        engine.Run();
    }
}