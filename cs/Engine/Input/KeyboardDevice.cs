using Raylib_cs;

namespace Engine.Input;

public interface IInputDevice
{
    void Poll();
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
    public KeyboardAndMouseDevice()
    {
    }

    public override void Poll()
    {
        foreach (var context in _contexts)
        {
            foreach (var mapping in context.IsPressedMappings)
            {
                if (Raylib.IsKeyPressed(mapping.Primary))
                {
                    Emit(mapping.Action, 1);
                }
            }

            foreach (var mapping in context.IsDownMappings)
            {
                if (Raylib.IsKeyDown(mapping.Primary))
                {
                    Emit(mapping.Action, 1);
                }
            }
        }
    }

    private void Emit(InputAction action, float value)
    {
        var inputEvent = new InputEvent(action, value);
        Console.WriteLine($"Input Event: {inputEvent.Action.Name}, Value: {inputEvent.Value}");
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
    public readonly KeyboardInputMapping[] IsDownMappings;
    public readonly KeyboardInputMapping[] IsPressedMappings;

    public KeyboardInputContext(
        KeyboardInputMapping[] isDownMappings,
        KeyboardInputMapping[] isPressedMappings)
    {
        IsDownMappings = isDownMappings;
        IsPressedMappings = isPressedMappings;
    }
}

public record struct KeyboardInputMapping(KeyboardKey Primary, InputAction Action);


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