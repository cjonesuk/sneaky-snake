using Engine;

namespace SneakySnake;

internal class GameSystem : ISystem
{
    private IGameEngine? _engine;
    private IGame? _game;

    public GameSystem()
    {
    }

    public void Attached(IGameEngine engine)
    {
        Console.WriteLine("GameSystem attached to engine.");
        _engine = engine;
        _game = new SneakySnakeGame(_engine);
    }

    public void Detached()
    {
        _engine = null;
        _game = null;
    }

    public void Update(float deltaTime)
    {
        _game?.Update(deltaTime);
    }
}
