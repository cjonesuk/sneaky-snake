namespace Engine.WorldManagement.Actors;

public interface IActorManager
{
    void Tick(float deltaTime);


    TActor SpawnActor<TActor, TProperties>(TProperties properties) where TActor : IActor<TProperties>, new();
}
