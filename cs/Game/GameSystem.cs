using Engine;

namespace SneakySnake;

internal class GameManager : IGameEngineObserver
{
    private IGameEngine? _engine;
    private IGame? _game;

    public GameManager()
    {
    }

    public void OnEngineStart(IGameEngine engine)
    {
        Console.WriteLine("Game Engine Started");
        _engine = engine;
        _game = new SneakySnakeGame(_engine);
    }

    public void OnEngineStop(IGameEngine engine)
    {
        _engine = null;
        _game = null;
    }

    public void Update(IGameEngine engine, float deltaTime)
    {
        _game?.Update(deltaTime);
    }
}
