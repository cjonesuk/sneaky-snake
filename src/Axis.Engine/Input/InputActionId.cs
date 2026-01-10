using System.Diagnostics;
using Axis.ECS;

namespace Axis.Engine.Input;

[DebuggerDisplay("{_value}")]
public readonly struct InputActionId : IEquatable<InputActionId>
{
    private readonly int _value;

    public int Value => _value;


    public static readonly Id Invalid = new Id();

    public InputActionId(int value)
    {
        _value = value;
    }

    public bool Equals(InputActionId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is InputActionId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value;
    }

    public static bool operator ==(InputActionId left, InputActionId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InputActionId left, InputActionId right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"InputActionId({_value})";
    }
}