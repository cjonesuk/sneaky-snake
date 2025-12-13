namespace SneakySnake;

interface ISystem
{
    void Attached(IEngine engine);
    void Detached();

    void Update(float deltaTime);
}
