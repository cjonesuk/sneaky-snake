using System.Numerics;
using Raylib_cs;

namespace Axis.Engine.Components;

public enum Shape3dKind
{
    Cube,
    Plane,
    Sphere,
    Cylinder,
}

public struct BasicShape3d
{
    public Shape3dKind Kind;
    public Vector3 Size;
    public Color Color;

    public BasicShape3d(Shape3dKind kind, Vector3 size, Color color)
    {
        Kind = kind;
        Size = size;
        Color = color;
    }
}
