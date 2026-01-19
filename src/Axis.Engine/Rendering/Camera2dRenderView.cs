
using System.Numerics;
using System.Runtime.InteropServices;

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
    // Placeholder for future 3D rendering parameters
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

    public static RenderMode Create3d(RenderMode3d mode3d)
    {
        return new RenderMode(RenderType.Render3d, default, mode3d);
    }

    public static RenderMode CreateScreenSpace()
    {
        return new RenderMode(RenderType.ScreenSpace, default, default);
    }
}