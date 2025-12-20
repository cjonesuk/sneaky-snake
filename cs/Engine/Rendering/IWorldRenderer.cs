namespace Engine.Rendering;

public interface IWorldRenderer
{
    void Generate(IWorld world, EntityId camera, IRenderPass renderPass);
}
