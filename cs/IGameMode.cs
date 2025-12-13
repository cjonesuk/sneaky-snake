namespace SneakySnake;

interface IGameMode
{
    void Enable();
    void Disable();
    void Update(float deltaTime);
}
