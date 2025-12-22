namespace Engine.Rendering;

public interface IWorld
{
    IEntityComponentManager Entities { get; }
    RenderPass CreateRenderPass();

    void Tick(float deltaTime);

    void GenerateRenderCommandsForCamera(IRenderPass renderPass, EntityId camera);
}
