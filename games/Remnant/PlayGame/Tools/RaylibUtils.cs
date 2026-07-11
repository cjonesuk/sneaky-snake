using Axis.Engine.Components;
using Raylib_cs;

namespace Remnant.PlayGame.Tools;

internal static class RaylibUtils
{
    public static Camera3D ToRaylibCamera(Camera3d camera)
    {
        return new Camera3D(
            camera.Position,
            camera.Target,
            camera.Up,
            camera.FovYDegrees,
            camera.Projection == Camera3dProjection.Orthographic
                ? CameraProjection.Orthographic
                : CameraProjection.Perspective);
    }
}
