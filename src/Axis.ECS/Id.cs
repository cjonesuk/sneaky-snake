using System.Runtime.CompilerServices;

namespace Axis.ECS;

public readonly struct Id : IEquatable<Id>
{
    private readonly ulong _id;

    public ulong Value => _id;

    public static readonly Id Invalid = new Id();

    public Id(uint id)
    {
        _id = id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return _id != 0; // tbd
    }

    public bool HasFlags(ulong flags)
    {
        return (_id & Constants.IdFlagsMask) == flags;
    }

    public bool Equals(Id other)
    {
        return _id == other._id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Id other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
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