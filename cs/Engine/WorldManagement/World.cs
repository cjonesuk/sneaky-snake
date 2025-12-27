using Engine.WorldManagement.Actors;
using Engine.WorldManagement.Entities;
using Engine.WorldManagement.Events;

namespace Engine.WorldManagement;

internal sealed class World : IWorld
{
    private readonly IEntityComponentManager _entities;
    private readonly IActorManager _actors;
    private readonly IEventManager _events;
    private readonly IWorldSystem[] _systems;
    private readonly IRenderer[] _renderers;
    private readonly IWorldRenderer _worldToRenderPass;

    public World(
        IWorldSystem[] systems,
        IRenderer[] renderers,
        IWorldRenderer worldToRenderPass)
    {
        _entities = new EntityComponentManager();
        _actors = new ActorManager(this);
        _events = new EventManager();
        _systems = systems;
        _renderers = renderers;
        _worldToRenderPass = worldToRenderPass;
    }

    public IEntityComponentManager Entities => _entities;
    public IActorManager Actors => _actors;
    public IEventManager Events => _events;

    public RenderPass CreateRenderPass()
    {
        return new RenderPass(_renderers);
    }

    public void Tick(float deltaTime)
    {
        _events.ClearAllEvents();

        _entities.ProcessPendingCommands();

        _actors.Tick(deltaTime);

        _entities.ProcessPendingCommands();

        foreach (var system in _systems)
        {
            system.Update(this, deltaTime);
        }

        _events.Debug();
    }

    public void GenerateRenderCommandsForCamera(IRenderPass renderPass, EntityId camera)
    {
        _worldToRenderPass.Generate(this, camera, renderPass);
    }
}
