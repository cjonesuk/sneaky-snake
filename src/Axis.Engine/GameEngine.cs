namespace Axis.Engine;

public interface IGameEngine
{

}

public sealed class GameEngine : IGameEngine
{
    internal GameEngine()
    {
    }

    public static GameEngine Create()
    {
        return new GameEngine();
    }

    public void Run()
    {

    }

}