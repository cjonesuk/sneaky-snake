using System.Numerics;

namespace Axis.Engine.Components;

public struct Transform3d
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public Transform3d(Vector3 position)
    {
        Position = position;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    public Transform3d(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}
