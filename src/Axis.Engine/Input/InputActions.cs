using System.Collections.Concurrent;

namespace Axis.Engine.Input;

public static class InputActions
{
    private static readonly ConcurrentDictionary<int, InputAction> _actionById;
    private static readonly ConcurrentDictionary<(string, string), InputAction> _actionByKey = new();

    static InputActions()
    {
        _actionById = new ConcurrentDictionary<int, InputAction>();
    }

    public static InputAction Register(string category, string name)
    {
        var id = ComputeId(category, name);
        var action = new InputAction(category, name, id);

        if (!_actionById.TryAdd(id.Value, action))
        {
            throw new InvalidOperationException($"Input action '{category}.{name}' is already registered.");
        }

        _actionByKey[(category, name)] = action;

        return action;
    }

    private static InputActionId ComputeId(string category, string name)
    {
        var id = HashCode.Combine(category, name);
        return new InputActionId(id);
    }

}
