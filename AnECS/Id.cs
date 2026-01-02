namespace AnECS;

public readonly struct Id : IEquatable<Id>
{
    private readonly IWorld _world;
    private readonly ulong _id;

    public IWorld World => _world;
    public ulong Value => _id;

    public static readonly Id Invalid = new Id();

    public Id(IWorld world, uint id)
    {
        _world = world;
        _id = id;
    }

    public bool HasFlags(ulong flags)
    {
        return (_id & Constants.IdFlagsMask) == flags;
    }

    public bool Equals(Id other)
    {
        return _world == other._world && _id == other._id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Id other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_world, _id);
    }

    public static bool operator ==(Id left, Id right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Id left, Id right)
    {
        return !(left == right);
    }

}