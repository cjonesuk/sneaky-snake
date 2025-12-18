
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
    public readonly Color ClearColor;

    public Viewport(
        float x,
        float y,
        float width,
        float height,
        IWorld world,
        EntityId camera,
        Color clearColor)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        World = world;
        Camera = camera;
        ClearColor = clearColor;
    }

}