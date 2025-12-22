using Engine.Rendering;

namespace Engine;

public interface IWorldSystem
{
    void Update(IWorld world, float deltaTime);
}
