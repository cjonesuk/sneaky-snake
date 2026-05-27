using System.Numerics;

namespace Axis.Engine.Components;

public enum Camera3dProjection
{
    Perspective,
    Orthographic,
}

public struct Camera3d
{
    public Vector3 Position;
    public Vector3 Target;
    public Vector3 Up;
    public float FovYDegrees;
    public Camera3dProjection Projection;

    public Camera3d(Vector3 position, Vector3 target, float fovYDegrees, Camera3dProjection projection)
    {
        Position = position;
        Target = target;
        Up = Vector3.UnitY;
        FovYDegrees = fovYDegrees;
        Projection = projection;
    }
}
