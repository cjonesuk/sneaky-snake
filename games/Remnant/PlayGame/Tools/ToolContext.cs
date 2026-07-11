using Axis.ECS;
using Axis.Engine.Input;
using Remnant.PlayGame.Collision;

namespace Remnant.PlayGame.Tools;

internal sealed class ToolContext
{
    internal ToolContext(
        IWorld world,
        ICollisionWorld collisionWorld,
        Entity camera,
        MouseState mouseState,
        ITool defaultTool)
    {
        World = world;
        CollisionWorld = collisionWorld;
        Camera = camera;
        MouseState = mouseState;
        DefaultTool = defaultTool;
        ActiveTool = defaultTool;
    }

    public IWorld World { get; }
    public ICollisionWorld CollisionWorld { get; }
    public Entity Camera { get; private set; }
    public MouseState MouseState { get; }
    public PointerRay PointerRay { get; private set; }
    public ITool ActiveTool { get; private set; }
    public ITool DefaultTool { get; }

    public void SetCamera(Entity camera)
    {
        Camera = camera;
    }

    public void SetPointerRay(PointerRay pointerRay)
    {
        PointerRay = pointerRay;
    }

    public void SwitchTool(ITool tool)
    {
        ActiveTool.OnDeactivate(this);
        ActiveTool = tool;
        ActiveTool.OnActivate(this);
    }

    public void ResetToDefaultTool()
    {
        SwitchTool(DefaultTool);
    }

}
