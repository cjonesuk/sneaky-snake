using Axis.Engine;

namespace Remnant;

static class Program
{
    [STAThread]
    public static void Main()
    {
        var settings = new Settings(1280, 720, "Remnant");

        var engine = GameEngine.Create(settings);
        var game = new RemnantGame();
        engine.Run(game);
    }
}
