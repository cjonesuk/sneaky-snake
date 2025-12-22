using Engine;

namespace SneakySnake;

internal enum GameState
{
    StartMenu,
    Playing,
}

internal class SneakySnakeGame : IGameInstance
{
    private readonly IGameEngine _engine;
    private IGameMode? _gameMode;
    private GameState? _currentState;
    private GameState? _nextState;

    public SneakySnakeGame(IGameEngine engine)
    {
        _engine = engine;
        _currentState = null;
        _nextState = null;
    }

    public void Initialise()
    {
        SetNextState(GameState.StartMenu);
    }

    public void Update(float deltaTime)
    {
        if (_nextState.HasValue)
        {
            if (_currentState.HasValue)
            {
                Console.WriteLine("Disabling current game mode...");
                _gameMode?.OnDeactivate();
                _gameMode = null;
                _currentState = null;
            }
            else
            {
                Console.WriteLine($"Switching to new game state: {_nextState.Value}");

                _currentState = _nextState;
                _nextState = null;
                _gameMode = CreateGameMode(_currentState.Value);

                Console.WriteLine("Enabling new game mode...");
                _gameMode.OnActivate();
            }

            return;
        }

        _gameMode?.Update(deltaTime);
    }

    private void SetNextState(GameState state)
    {
        _nextState = state;
    }

    public void StartGame()
    {
        SetNextState(GameState.Playing);
    }

    public void EndGame()
    {
        SetNextState(GameState.StartMenu);
    }

    private IGameMode CreateGameMode(GameState state)
    {
        return state switch
        {
            GameState.StartMenu => new StartMenuMode(this, _engine),
            GameState.Playing => new PlayMode(this, _engine),
            _ => throw new ArgumentOutOfRangeException(nameof(state), $"Unsupported game state: {state}"),
        };
    }
}
