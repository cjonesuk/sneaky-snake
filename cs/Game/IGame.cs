namespace SneakySnake;

/// <summary>
/// Interface for the single game instance which is the orchestration point for the game.
/// </summary> 
interface IGameInstance
{
    void Initialise();

    void Update(float deltaTime);


    // Starts a new game
    void StartGame();

    // Ends the current game, returning to the main menu
    void EndGame();
}
