namespace Axis.Engine.Input;

// public sealed class GamepadDevice : InputDevice
// {
//     public GamepadDevice()
//     {
//     }
// }

/// <summary>
/// Maps from input device state to game actions.
/// </summary>
public readonly struct KeyboardInputContext
{
    public readonly IInputReceiver Receiver;
    public readonly KeyboardInputMapping[] KeyDown;
    public readonly KeyboardInputMapping[] KeyPressed;
    public readonly MouseWheelInputMapping[] MouseWheel;

    public KeyboardInputContext(
        IInputReceiver receiver,
        KeyboardInputMapping[] keyDown,
        KeyboardInputMapping[] keyPressed,
        MouseWheelInputMapping[]? mouseWheel = null)
    {
        Receiver = receiver;
        KeyDown = keyDown;
        KeyPressed = keyPressed;
        MouseWheel = mouseWheel ?? Array.Empty<MouseWheelInputMapping>();
    }
}
