using Engine;

namespace SneakySnake;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Settings settings = new Settings(800, 600, "Sneaky Snake");
        IGameEngine engine = new GameEngine(settings);

        ISystem[] systems = {
            new GameSystem(),
        };

        engine.Run(systems);
    }
}