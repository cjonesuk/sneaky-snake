using Engine.WorldManagement.Actors;
using Engine.WorldManagement.Entities;

namespace Engine.WorldManagement;

public interface IWorld
{
    IEntityComponentManager Entities { get; }
    IActorManager Actors { get; }
    RenderPass CreateRenderPass();

    void Tick(float deltaTime);

    void GenerateRenderCommandsForCamera(IRenderPass renderPass, EntityId camera);
}
