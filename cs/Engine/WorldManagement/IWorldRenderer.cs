using Engine.WorldManagement.Entities;

namespace Engine.WorldManagement;

public interface IWorldRenderer
{
    void Generate(IWorld world, EntityId camera, IRenderPass renderPass);
}
