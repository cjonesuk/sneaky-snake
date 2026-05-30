using System.Numerics;
using Raylib_cs;

namespace Axis.Engine.Input;

/// <summary>Per-frame snapshot of mouse position and button edges. Updated by the input device.</summary>
public sealed class MouseState
{
    private uint _down;
    private uint _pressed;
    private uint _released;

    /// <summary>Cursor position in screen pixels.</summary>
    public Vector2 Position { get; private set; }

    /// <summary>Movement delta since the previous frame.</summary>
    public Vector2 Delta { get; private set; }

    /// <summary>True if any UI element claimed the cursor this frame.</summary>
    public bool IsCapturedByUI { get; private set; }

    /// <summary>True while the button is held down.</summary>
    public bool IsDown(MouseButton button) => (_down & Mask(button)) != 0u;

    /// <summary>True only on the frame the button transitioned from up to down.</summary>
    public bool WasPressed(MouseButton button) => (_pressed & Mask(button)) != 0u;

    /// <summary>True only on the frame the button transitioned from down to up.</summary>
    public bool WasReleased(MouseButton button) => (_released & Mask(button)) != 0u;

    /// <summary>Mark the cursor as consumed by UI for the rest of this frame.</summary>
    public void Capture()
    {
        IsCapturedByUI = true;
    }

    internal void BeginFrame(Vector2 position, uint down, uint pressed, uint released)
    {
        Delta = position - Position;
        Position = position;
        _down = down;
        _pressed = pressed;
        _released = released;
        IsCapturedByUI = false;
    }

    private static uint Mask(MouseButton button) => 1u << (int)button;
}
