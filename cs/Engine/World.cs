namespace Engine.Rendering;

internal sealed class World : IWorld
{
    private readonly IEntityComponentManager _entities;
    private readonly IWorldSystem[] _systems;
    private readonly IRenderer[] _renderers;
    private readonly IWorldRenderer _worldToRenderPass;

    public World(
        IEntityComponentManager entities,
        IWorldSystem[] systems,
        IRenderer[] renderers,
        IWorldRenderer worldToRenderPass)
    {
        _entities = entities;
        _systems = systems;
        _renderers = renderers;
        _worldToRenderPass = worldToRenderPass;
    }

    public IEntityComponentManager Entities => _entities;

    public RenderPass CreateRenderPass()
    {
        return new RenderPass(_renderers);
    }

    public void Tick(float deltaTime)
    {
        _entities.ProcessPendingCommands();

        foreach (var system in _systems)
        {
            system.Update(this, deltaTime);
        }
    }

    public void GenerateRenderCommandsForCamera(IRenderPass renderPass, EntityId camera)
    {
        _worldToRenderPass.Generate(this, camera, renderPass);
    }
}
