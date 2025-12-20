using System.Numerics;
using Raylib_cs;

namespace Engine.Input;

public interface IDevice
{

}

internal sealed class KeyboardDevice : IDevice
{
    public KeyboardDevice()
    {
    }
}

public sealed class MouseDevice : IDevice
{
    public MouseDevice()
    {
    }
}

// public sealed class GamepadDevice : IDevice
// {
//     public GamepadDevice()
//     {
//     }
// }

/// <summary>
/// Maps from input device state to game actions.
/// </summary>
public sealed class InputContext
{
    private readonly IDevice[] _devices;


    public InputContext(IDevice[] devices)
    {
        _devices = devices;
    }

    public void Process()
    {

    }
}

public record struct InputMapping(InputAction Action);


public abstract class InputAction
{
    public abstract string Name { get; }
}

public sealed class MoveForwardAction : InputAction
{
    public override string Name => "MoveForward";
    public static readonly MoveForwardAction Instance = new();
}

public readonly struct InputEvent
{
    public InputAction Action { get; }
    public Vector2 Value { get; }

    public InputEvent(InputAction action, Vector2 value)
    {
        Action = action;
        Value = value;
    }
}