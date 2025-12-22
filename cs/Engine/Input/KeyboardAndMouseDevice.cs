using Raylib_cs;

namespace Engine.Input;

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
