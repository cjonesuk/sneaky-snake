using System.Diagnostics;
using Axis.Engine;
namespace PingPong;

internal sealed class PingPongGame : IGameInstance
{
    private IGameEngine? _engine;

    public void OnEngineStart(IGameEngine engine)
    {
        Debug.Assert(_engine == null);

        _engine = engine;
        Console.WriteLine("PingPong Game Started");
    }

    public void OnEngineStop(IGameEngine engine)
    {
        Debug.Assert(_engine == engine);

        _engine = null;
        Console.WriteLine("PingPong Game Stopped");
    }
}