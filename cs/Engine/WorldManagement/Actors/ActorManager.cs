namespace Engine.WorldManagement.Actors;

internal sealed class ActorManager : IActorManager
{
    private readonly IWorld _world;
    private readonly List<IActor> _actors = new();

    public ActorManager(IWorld world)
    {
        _world = world;
    }

    public TActor SpawnActor<TActor, TProperties>(TProperties properties) where TActor : IActor<TProperties>, new()
    {
        var actor = new TActor();

        actor.Initialize(_world, properties);

        _actors.Add(actor);

        return actor;
    }

    public void Tick(float deltaTime)
    {
        foreach (var actor in _actors)
        {
            actor.Tick(deltaTime);
        }
    }
}
