using Axis.Engine;
namespace PingPong;

static class Program
{
    [STAThread]
    public static void Main()
    {
        var settings = new Settings(800, 600, "Ping Pong");

        var engine = GameEngine.Create(settings);
        var game = new PingPongGame();
        engine.Run(game);
    }
}