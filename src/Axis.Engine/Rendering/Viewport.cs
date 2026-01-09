
using Axis.ECS;

namespace Axis.Engine.Rendering;

public readonly struct Viewport
{
    public readonly float X;
    public readonly float Y;
    public readonly float Width;
    public readonly float Height;
    public readonly Entity Camera;

    public Viewport(
        float x,
        float y,
        float width,
        float height,
        Entity camera)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Camera = camera;
    }

    public static Viewport Fullscreen(Entity camera)
    {
        return new Viewport(0.0f, 0.0f, 1.0f, 1.0f, camera);
    }

    public static Viewport[] SplitColumns(Entity cameraLeft, Entity cameraRight)
    {
        return
        [
            new Viewport(0.0f, 0.0f, 0.5f, 1.0f, cameraLeft),
            new Viewport(0.5f, 0.0f, 0.5f, 1.0f, cameraRight)
        ];
    }


}