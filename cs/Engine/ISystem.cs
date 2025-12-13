namespace Engine;

public interface ISystem
{
    void Attached(IGameEngine engine);
    void Detached();

    void Update(float deltaTime);
}
