using Engine;

namespace SneakySnake;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        // todo: handle resizable windows
        Settings settings = new Settings(800, 600, "Sneaky Snake");
        IGameEngine engine = new GameEngine(settings);

        IGameEngineObserver[] observers = {
            new GameManager(),
        };

        engine.Run(observers);
    }
}