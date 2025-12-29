using Engine.Input;

namespace Engine.Components;

internal readonly struct InputActionReceiver
{
    public readonly List<InputAction> PendingActions;

    public InputActionReceiver(List<InputAction> pendingActions)
    {
        PendingActions = pendingActions;
    }
    public static InputActionReceiver Create()
    {
        return new InputActionReceiver(new List<InputAction>());
    }
}