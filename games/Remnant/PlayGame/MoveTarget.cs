using System.Numerics;

namespace Remnant.PlayGame;

/// <summary>Destination an entity is moving toward. Removed when the entity arrives.</summary>
public struct MoveTarget
{
    public Vector3 Position;

    public MoveTarget(Vector3 position)
    {
        Position = position;
    }
}
