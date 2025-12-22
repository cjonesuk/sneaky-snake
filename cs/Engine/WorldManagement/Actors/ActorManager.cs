namespace Engine.WorldManagement.Actors;

internal sealed class ActorManager : IActorManager
{
    private readonly IWorld _world;
    private readonly List<IActor> _actors = new();

    public ActorManager(IWorld world)
    {
        _world = world;
    }

    public void AddActor(IActor actor)
    {
        _actors.Add(actor);
        actor.OnAttached(_world);
    }

    public void Tick(float deltaTime)
    {
        foreach (var actor in _actors)
        {
            actor.Tick(deltaTime);
        }
    }
}
