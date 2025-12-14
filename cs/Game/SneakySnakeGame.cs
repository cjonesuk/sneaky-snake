using Engine;
using Raylib_cs;
using SneakySnake.Game;

namespace SneakySnake;

internal class SneakySnakeGame : IGame
{
    private readonly IGameEngine _engine;
    private IGameMode? _gameMode;

    public SneakySnakeGame(IGameEngine engine)
    {
        _engine = engine;
    }

    public void Initialise()
    {
        _engine.SetSystems([
            new GridMovementSystem(),
            new BasicRenderSystem(Color.SkyBlue)
        ]);

        SwitchMode(new StartMenuMode(this, _engine));
    }

    public void Update(float deltaTime)
    {
        _gameMode?.Update(deltaTime);
    }

    private void SwitchMode(IGameMode newMode)
    {
        _gameMode?.Disable();
        _gameMode = newMode;
        _gameMode.Enable();
    }

    public void StartGame()
    {
        SwitchMode(new PlayMode(this, _engine));
    }

    public void EndGame()
    {
        SwitchMode(new StartMenuMode(this, _engine));
    }
}
