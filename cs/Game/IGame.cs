namespace SneakySnake;

interface IGame
{
    void Initialise();

    void Update(float deltaTime);


    // Starts a new game
    void StartGame();

    // Ends the current game, returning to the main menu
    void EndGame();
}
