using System.Numerics;
using Raylib_cs;

namespace Axis.Engine.Input;

/// <summary>
/// Represents a keyboard and mouse as a single input device.
/// </summary>
internal sealed class KeyboardAndMouseDevice : InputDevice<KeyboardInputContext>, IKeyboardAndMouseDevice
{
    private static readonly MouseButton[] _polledButtons =
    {
        MouseButton.Left,
        MouseButton.Right,
        MouseButton.Middle,
    };

    private readonly HashSet<KeyboardKey> _blockedKeys = new();
    private readonly HashSet<MouseButton> _blockedButtons = new();
    private readonly MouseState _mouse = new();

    public KeyboardAndMouseDevice()
    {
    }

    public MouseState Mouse => _mouse;

    public override void Poll()
    {
        _blockedKeys.Clear();
        _blockedButtons.Clear();

        Vector2 position = Raylib.GetMousePosition();
        float wheelDelta = Raylib.GetMouseWheelMove();

        uint down = 0u;
        uint pressed = 0u;
        uint released = 0u;
        foreach (var button in _polledButtons)
        {
            uint mask = 1u << (int)button;
            if (Raylib.IsMouseButtonDown(button)) down |= mask;
            if (Raylib.IsMouseButtonPressed(button)) pressed |= mask;
            if (Raylib.IsMouseButtonReleased(button)) released |= mask;
        }
        _mouse.BeginFrame(position, down, pressed, released);

        foreach (var context in _contexts)
        {
            DispatchKeyboard(context);
            DispatchMouseWheel(context, wheelDelta);
            DispatchMouseButtons(context);
        }
    }

    private void DispatchKeyboard(in KeyboardInputContext context)
    {
        foreach (var mapping in context.KeyPressed)
        {
            if (_blockedKeys.Contains(mapping.Primary)) continue;
            if (!Raylib.IsKeyPressed(mapping.Primary)) continue;

            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

            if (mapping.IsBlocking) _blockedKeys.Add(mapping.Primary);
        }

        foreach (var mapping in context.KeyDown)
        {
            if (_blockedKeys.Contains(mapping.Primary)) continue;
            if (!Raylib.IsKeyDown(mapping.Primary)) continue;

            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

            if (mapping.IsBlocking) _blockedKeys.Add(mapping.Primary);
        }
    }

    private static void DispatchMouseWheel(in KeyboardInputContext context, float wheelDelta)
    {
        if (wheelDelta == 0f) return;

        var direction = wheelDelta > 0f ? MouseWheelDirection.Up : MouseWheelDirection.Down;
        float magnitude = MathF.Abs(wheelDelta);

        foreach (var mapping in context.MouseWheel)
        {
            if (mapping.Direction != direction) continue;
            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, magnitude));
        }
    }

    private void DispatchMouseButtons(in KeyboardInputContext context)
    {
        foreach (var mapping in context.MouseButtonPressed)
        {
            if (_blockedButtons.Contains(mapping.Primary)) continue;
            if (!_mouse.WasPressed(mapping.Primary)) continue;

            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

            if (mapping.IsBlocking) _blockedButtons.Add(mapping.Primary);
        }

        foreach (var mapping in context.MouseButtonReleased)
        {
            if (_blockedButtons.Contains(mapping.Primary)) continue;
            if (!_mouse.WasReleased(mapping.Primary)) continue;

            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

            if (mapping.IsBlocking) _blockedButtons.Add(mapping.Primary);
        }

        foreach (var mapping in context.MouseButtonDown)
        {
            if (_blockedButtons.Contains(mapping.Primary)) continue;
            if (!_mouse.IsDown(mapping.Primary)) continue;

            context.Receiver.ReceiveInput(new InputEvent(mapping.ActionId, 1));

            if (mapping.IsBlocking) _blockedButtons.Add(mapping.Primary);
        }
    }
}
