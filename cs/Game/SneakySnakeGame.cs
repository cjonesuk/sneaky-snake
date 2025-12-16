using Engine;
using Raylib_cs;
using SneakySnake.Game;

namespace SneakySnake;

internal enum GameState
{
    StartMenu,
    Playing,
}

internal class SneakySnakeGame : IGame
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
        _engine.SetSystems([
            new GridMovementSystem(),
            new BasicRenderSystem(Color.SkyBlue)
        ]);

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
                _engine.Entities.NewWorld();
            }
            else
            {
                Console.WriteLine($"Switching to new game state: {_nextState.Value}");
                _currentState = _nextState;
                _nextState = null;

                switch (_currentState)
                {
                    case GameState.StartMenu:
                        _gameMode = new StartMenuMode(this, _engine);
                        break;
                    case GameState.Playing:
                        _gameMode = new PlayMode(this, _engine);
                        break;
                }

                Console.WriteLine("Enabling new game mode...");
                _gameMode?.OnActivate();
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
}
