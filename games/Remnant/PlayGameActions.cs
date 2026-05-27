using Axis.Engine.Input;

namespace Remnant;

using static Axis.Engine.Input.InputActions;

internal static class PlayGameActions
{
    public static readonly string Category = "PlayGame";

    public static readonly InputAction PanUp = Register(Category, nameof(PanUp));
    public static readonly InputAction PanDown = Register(Category, nameof(PanDown));
    public static readonly InputAction PanLeft = Register(Category, nameof(PanLeft));
    public static readonly InputAction PanRight = Register(Category, nameof(PanRight));
    public static readonly InputAction ZoomIn = Register(Category, nameof(ZoomIn));
    public static readonly InputAction ZoomOut = Register(Category, nameof(ZoomOut));
}
