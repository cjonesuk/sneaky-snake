using Engine.Input;

namespace SneakySnake;

public static class SnakeActions
{
    public sealed class MoveForward : InputAction
    {
        public override string Name => "MoveForward";
        public static readonly MoveForward Instance = new();
    }

    public sealed class SlowDown : InputAction
    {
        public override string Name => "SlowDown";
        public static readonly SlowDown Instance = new();
    }

    public sealed class TurnLeft : InputAction
    {
        public override string Name => "TurnLeft";
        public static readonly TurnLeft Instance = new();
    }

    public sealed class TurnRight : InputAction
    {
        public override string Name => "TurnRight";
        public static readonly TurnRight Instance = new();
    }
}
