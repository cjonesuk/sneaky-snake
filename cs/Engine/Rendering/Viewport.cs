
using Raylib_cs;

namespace Engine.Rendering;

public readonly struct Viewport
{
    public readonly float X;
    public readonly float Y;
    public readonly float Width;
    public readonly float Height;
    public readonly IWorld World;
    public readonly EntityId Camera;

    public Viewport(
        float x,
        float y,
        float width,
        float height,
        IWorld world,
        EntityId camera)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        World = world;
        Camera = camera;
    }

    public static Viewport Fullscreen(IWorld world, EntityId camera)
    {
        return new Viewport(0.0f, 0.0f, 1.0f, 1.0f, world, camera);
    }

    public static Viewport[] SplitColumns(IWorld world, EntityId cameraLeft, EntityId cameraRight)
    {
        return
        [
            new Viewport(0.0f, 0.0f, 0.5f, 1.0f, world, cameraLeft),
            new Viewport(0.5f, 0.0f, 0.5f, 1.0f, world, cameraRight)
        ];
    }


}