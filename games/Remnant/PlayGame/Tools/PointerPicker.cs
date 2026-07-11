using System.Numerics;
using Axis.Engine.Components;
using Raylib_cs;

namespace Remnant.PlayGame.Tools;

internal readonly record struct PointerRay(
    Ray Ray,
    bool HasGroundHit,
    Vector3 GroundHitPoint);


internal static class PointerPicker
{
    public static PointerRay Compute(in Camera3d camera, Vector2 screenPosition)
    {
        Camera3D rayCamera = RaylibUtils.ToRaylibCamera(camera);
        Ray ray = Raylib.GetScreenToWorldRay(screenPosition, rayCamera);

        // Ray is pointing up or level
        if (ray.Direction.Y >= 0f)
        {
            return new PointerRay(ray, false, Vector3.Zero);
        }

        float t = -ray.Position.Y / ray.Direction.Y;

        // Ray is behind the rays origin
        if (t < 0f)
        {
            return new PointerRay(ray, false, Vector3.Zero);
        }

        Vector3 groundHit = ray.Position + ray.Direction * t;
        return new PointerRay(ray, true, groundHit);
    }
}
