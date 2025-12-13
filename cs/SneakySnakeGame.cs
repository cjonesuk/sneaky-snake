namespace SneakySnake;

internal class SneakySnakeGame : IGame
{
    private readonly IEngine _engine;
    private IGameMode? _gameMode;

    public SneakySnakeGame(IEngine engine)
    {
        _engine = engine;
        SwitchMode(new StartMenuMode(this, engine));
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
