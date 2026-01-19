using Axis.Engine.Input;
namespace PingPong;

using static Axis.Engine.Input.InputActions;

internal static class PlayGameActions
{
    public static readonly string Category = "PlayGame";

    public static readonly InputAction QuitGame = Register(Category, nameof(QuitGame));
    public static readonly InputAction MovePaddleUp = Register(Category, nameof(MovePaddleUp));
    public static readonly InputAction MovePaddleDown = Register(Category, nameof(MovePaddleDown));
}