using Raylib_cs;

namespace Engine.Input;

public interface IInputDevice
{
    void Poll();
    void ClearContext();
}

public interface IKeyboardAndMouseDevice : IInputDevice
{
    void BindContext(params KeyboardInputContext[] contexts);
}

public abstract class InputDevice<TContext> : IInputDevice
{
    protected TContext[] _contexts;

    protected InputDevice()
    {
        _contexts = Array.Empty<TContext>();
    }

    public virtual void ClearContext()
    {
        _contexts = Array.Empty<TContext>();
    }

    public virtual void BindContext(params TContext[] contexts)
    {
        _contexts = contexts;
    }

    public virtual void Poll()
    {
        // Default implementation does nothing.
    }
}

/// <summary>
/// Represents a keyboard and mouse as a single input device.
/// </summary>
internal sealed class KeyboardAndMouseDevice : InputDevice<KeyboardInputContext>, IKeyboardAndMouseDevice
{
    private readonly HashSet<KeyboardKey> _blockedKeys = new();

    public KeyboardAndMouseDevice()
    {
    }

    public override void Poll()
    {
        _blockedKeys.Clear();

        foreach (var context in _contexts)
        {
            foreach (var mapping in context.KeyPressed)
            {
                if (_blockedKeys.Contains(mapping.Primary))
                {
                    continue;
                }

                if (Raylib.IsKeyPressed(mapping.Primary))
                {
                    // Console.WriteLine($"Key Pressed: {mapping.Primary} -> {mapping.Action.Name}");
                    var inputEvent = new InputEvent(mapping.Action, 1);
                    context.Receiver.ReceiveInput(inputEvent);

                    if (mapping.IsBlocking)
                    {
                        _blockedKeys.Add(mapping.Primary);
                    }
                }
            }

            foreach (var mapping in context.KeyDown)
            {
                if (_blockedKeys.Contains(mapping.Primary))
                {
                    continue;
                }

                if (Raylib.IsKeyDown(mapping.Primary))
                {
                    // Console.WriteLine($"Key Down: {mapping.Primary} -> {mapping.Action.Name}");
                    context.Receiver.ReceiveInput(new InputEvent(mapping.Action, 1));

                    if (mapping.IsBlocking)
                    {
                        _blockedKeys.Add(mapping.Primary);
                    }
                }
            }
        }
    }
}

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

public abstract class InputAction
{
    public abstract string Name { get; }
}


public readonly struct InputEvent
{
    public InputAction Action { get; }
    public float Value { get; }

    public InputEvent(InputAction action, float value)
    {
        Action = action;
        Value = value;
    }
}

public interface IInputReceiver
{
    void ReceiveInput(InputEvent inputEvent);
}