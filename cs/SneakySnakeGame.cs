namespace SneakySnake;

internal class SneakySnakeGame : IGame
{
    private readonly IEngine _engine;
    private IGameMode? _gameMode;

    public SneakySnakeGame(IEngine engine)
    {
        _engine = engine;
        SwitchMode(new StartMenuMode(engine));
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
        SwitchMode(new PlayMode());
    }

    public void EndGame()
    {
        SwitchMode(new StartMenuMode(_engine));
    }
}
