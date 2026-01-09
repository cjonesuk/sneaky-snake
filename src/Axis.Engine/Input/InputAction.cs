using System.Diagnostics;

namespace Axis.Engine.Input;

[DebuggerDisplay("{Category}.{Name} ({Id})")]
public sealed class InputAction
{
    public string Category { get; }
    public string Name { get; }

    public InputActionId Id { get; }

    public InputAction(string category, string name, InputActionId id)
    {
        Category = category;
        Name = name;
        Id = id;
    }

    public static implicit operator InputActionId(InputAction action) => action.Id;

    public override string ToString()
    {
        return $"{Category}.{Name})";
    }
}
