namespace Engine;

public interface IGameEngineObserver
{
    void OnEngineStart(IGameEngine engine);
    void OnEngineStop(IGameEngine engine);
    void Update(IGameEngine engine, float deltaTime);
}
