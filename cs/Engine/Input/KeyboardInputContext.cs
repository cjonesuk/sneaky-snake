namespace Engine.Input;

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

    public KeyboardInputContext(
        IInputReceiver receiver,
        KeyboardInputMapping[] keyDown,
        KeyboardInputMapping[] keyPressed)
    {
        Receiver = receiver;
        KeyDown = keyDown;
        KeyPressed = keyPressed;
    }
}
