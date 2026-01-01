using System;

namespace AnECS;




public interface IWorld
{

}

internal sealed class World : IWorld
{
    private uint _nextId = 1;

    internal World()
    {

    }

    public static World Create()
    {
        return new World();
    }

    public Entity Entity()
    {
        Id id = new Id(this, _nextId++);
        return new Entity(id);
    }
}

public readonly struct Entity
{
    private readonly Id _id;

    public Id Id => _id;

    public IWorld World => _id.World;

    public Entity(Id id)
    {
        _id = id;
    }
}

public static class Constants
{
    public const ulong IdFlagsMask = 0xF000_0000_0000_0000;
}

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