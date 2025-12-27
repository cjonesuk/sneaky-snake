using Engine.WorldManagement.Actors;
using Engine.WorldManagement.Entities;
using Engine.WorldManagement.Events;

namespace Engine.WorldManagement;

public interface IWorld
{
    IEntityComponentManager Entities { get; }
    IActorManager Actors { get; }
    IEventManager Events { get; }
    RenderPass CreateRenderPass();

    void Tick(float deltaTime);

    void GenerateRenderCommandsForCamera(IRenderPass renderPass, EntityId camera);
}
