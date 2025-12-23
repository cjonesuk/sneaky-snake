namespace Engine.WorldManagement.Actors;

public interface IActor
{
    void Tick(float deltaTime);
}

public interface IActor<TProperties> : IActor
{
    void Initialize(IWorld world, TProperties properties);
}