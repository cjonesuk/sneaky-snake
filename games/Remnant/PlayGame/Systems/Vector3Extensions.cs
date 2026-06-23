using System.Numerics;

namespace Remnant.PlayGame.Systems;

public static class Vector3Extensions
{
    extension(Vector3 v)
    {
        public Vector2 XZ() => new Vector2(v.X, v.Z);
        public Vector2 XY() => new Vector2(v.X, v.Y);
    }
}