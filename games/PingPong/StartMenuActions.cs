using Axis.Engine.Input;
namespace PingPong;

using static Axis.Engine.Input.InputActions;

internal static class StartMenuActions
{
    public static readonly string Category = "StartMenu";

    public static readonly InputAction StartGame = Register(Category, nameof(StartGame));
}