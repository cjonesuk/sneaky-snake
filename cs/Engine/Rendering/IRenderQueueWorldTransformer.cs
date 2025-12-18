namespace Engine.Rendering;

public interface IRenderQueueWorldTransformer
{
    void Generate(IWorld world, EntityId camera, IRenderPass renderPass);
}
