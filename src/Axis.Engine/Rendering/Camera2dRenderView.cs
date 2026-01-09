
using System.Numerics;

namespace Axis.Engine.Rendering;

public readonly struct Camera2dRenderView
{
    public readonly Vector2 Target;
    public readonly float Zoom;
    public readonly float Rotation;

    public Camera2dRenderView(Vector2 target, float zoom, float rotation)
    {
        Target = target;
        Zoom = zoom;
        Rotation = rotation;
    }

}
