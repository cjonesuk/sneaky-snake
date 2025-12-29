using Engine.Input;

namespace SneakySnake;

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