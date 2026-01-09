namespace Axis.Engine.Input;

public readonly struct InputEvent
{
    public InputActionId Id { get; }
    public float Value { get; }

    public InputEvent(InputActionId id, float value)
    {
        Id = id;
        Value = value;
    }
}
