using Raylib_cs;

namespace Engine.Input;

public readonly struct KeyboardInputMapping
{
    /// <summary>
    /// The primary keyboard key that triggers the action.
    /// </summary>
    public readonly KeyboardKey Primary;

    /// <summary>
    /// Whether this mapping blocks further mappings from being processed.
    /// </summary>
    public readonly bool IsBlocking;

    /// <summary>
    /// The action to trigger when the input condition is met.
    /// </summary>
    public readonly InputAction Action;

    public KeyboardInputMapping(KeyboardKey primary, InputAction action, bool isBlocking = false)
    {
        Primary = primary;
        Action = action;
        IsBlocking = isBlocking;
    }
}
