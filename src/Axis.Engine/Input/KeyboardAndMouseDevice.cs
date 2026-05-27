using Raylib_cs;

namespace Axis.Engine.Input;

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

        float wheelDelta = Raylib.GetMouseWheelMove();

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
                    var inputEvent = new InputEvent(mapping.ActionId, 1);
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
                    context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

                    if (mapping.IsBlocking)
                    {
                        _blockedKeys.Add(mapping.Primary);
                    }
                }
            }

            if (wheelDelta == 0f)
            {
                continue;
            }

            var direction = wheelDelta > 0f ? MouseWheelDirection.Up : MouseWheelDirection.Down;
            float magnitude = MathF.Abs(wheelDelta);

            foreach (var mapping in context.MouseWheel)
            {
                if (mapping.Direction != direction)
                {
                    continue;
                }

                context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, magnitude));
            }
        }
    }
}
