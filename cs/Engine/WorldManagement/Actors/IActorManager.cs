namespace Engine.WorldManagement.Actors;

public interface IActorManager
{
    void AddActor(IActor actor);
    void Tick(float deltaTime);
}
