
using System.Numerics;
using System.Runtime.InteropServices;
using Axis.Engine.Components;

namespace Axis.Engine.Rendering;

public readonly struct RenderMode2d
{
    public readonly Vector2 Target;
    public readonly float Zoom;
    public readonly float Rotation;

    public RenderMode2d(Vector2 target, float zoom, float rotation)
    {
        Target = target;
        Zoom = zoom;
        Rotation = rotation;
    }
}

public readonly struct RenderMode3d
{
    public readonly Vector3 Position;
    public readonly Vector3 Target;
    public readonly Vector3 Up;
    public readonly float FovYDegrees;
    public readonly Camera3dProjection Projection;

    public RenderMode3d(Vector3 position, Vector3 target, Vector3 up, float fovYDegrees, Camera3dProjection projection)
    {
        Position = position;
        Target = target;
        Up = up;
        FovYDegrees = fovYDegrees;
        Projection = projection;
    }
}

public enum RenderType { None, Render2d, Render3d, ScreenSpace }

[StructLayout(LayoutKind.Explicit)]
public readonly struct RenderMode
{
    [FieldOffset(0)]
    public readonly RenderType RenderType;

    [FieldOffset(4)]
    public readonly RenderMode2d Mode2d;

    [FieldOffset(4)]
    public readonly RenderMode3d Mode3d;

    public static readonly RenderMode None = new RenderMode(RenderType.None, default, default);

    private RenderMode(RenderType renderType, RenderMode2d mode2d, RenderMode3d mode3d)
    {
        RenderType = renderType;
        Mode2d = mode2d;
        Mode3d = mode3d;
    }

    public static RenderMode Create2d(Vector2 target, float zoom, float rotation)
    {
        var mode2d = new RenderMode2d(target, zoom, rotation);
        return new RenderMode(RenderType.Render2d, mode2d, default);
    }

    public static RenderMode Create3d(in Camera3d camera)
    {
        var mode3d = new RenderMode3d(camera.Position, camera.Target, camera.Up, camera.FovYDegrees, camera.Projection);
        return new RenderMode(RenderType.Render3d, default, mode3d);
    }

    public static RenderMode CreateScreenSpace()
    {
        return new RenderMode(RenderType.ScreenSpace, default, default);
    }
}
