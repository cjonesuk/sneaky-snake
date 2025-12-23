namespace Engine.Input;

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
