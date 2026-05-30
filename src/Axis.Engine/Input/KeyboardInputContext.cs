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
    public readonly MouseButtonInputMapping[] MouseButtonDown;
    public readonly MouseButtonInputMapping[] MouseButtonPressed;
    public readonly MouseButtonInputMapping[] MouseButtonReleased;

    public KeyboardInputContext(
        IInputReceiver receiver,
        KeyboardInputMapping[] keyDown,
        KeyboardInputMapping[] keyPressed,
        MouseWheelInputMapping[]? mouseWheel = null,
        MouseButtonInputMapping[]? mouseButtonDown = null,
        MouseButtonInputMapping[]? mouseButtonPressed = null,
        MouseButtonInputMapping[]? mouseButtonReleased = null)
    {
        Receiver = receiver;
        KeyDown = keyDown;
        KeyPressed = keyPressed;
        MouseWheel = mouseWheel ?? Array.Empty<MouseWheelInputMapping>();
        MouseButtonDown = mouseButtonDown ?? Array.Empty<MouseButtonInputMapping>();
        MouseButtonPressed = mouseButtonPressed ?? Array.Empty<MouseButtonInputMapping>();
        MouseButtonReleased = mouseButtonReleased ?? Array.Empty<MouseButtonInputMapping>();
    }
}
