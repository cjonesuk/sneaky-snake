namespace SneakySnake;

interface IGameMode
{
    void OnActivate();
    void OnDeactivate();
    void Update(float deltaTime);
}
