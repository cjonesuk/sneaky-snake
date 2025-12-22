namespace Engine.WorldManagement.Actors;

public interface IActor
{
    void OnAttached(IWorld world);
    void Tick(float deltaTime);
}
