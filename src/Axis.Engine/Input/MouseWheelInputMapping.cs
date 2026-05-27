namespace Axis.Engine.Input;

public enum MouseWheelDirection
{
    Up,
    Down,
}

public readonly struct MouseWheelInputMapping
{
    public readonly MouseWheelDirection Direction;
    public readonly InputActionId ActionId;

    public MouseWheelInputMapping(MouseWheelDirection direction, InputActionId actionId)
    {
        Direction = direction;
        ActionId = actionId;
    }
}
