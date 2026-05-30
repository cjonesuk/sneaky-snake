using Raylib_cs;

namespace Axis.Engine.Input;

public readonly struct MouseButtonInputMapping
{
    /// <summary>The mouse button that triggers the action.</summary>
    public readonly MouseButton Primary;

    /// <summary>Whether this mapping blocks further mappings from being processed.</summary>
    public readonly bool IsBlocking;

    /// <summary>The identifier of the action to trigger when the input condition is met.</summary>
    public readonly InputActionId ActionId;

    public MouseButtonInputMapping(MouseButton primary, InputActionId actionId, bool isBlocking = false)
    {
        Primary = primary;
        ActionId = actionId;
        IsBlocking = isBlocking;
    }
}
