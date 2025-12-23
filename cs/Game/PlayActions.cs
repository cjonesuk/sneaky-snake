using Engine.Input;

namespace SneakySnake;

public static class PlayActions
{
    public sealed class EndGame : InputAction
    {
        public override string Name => "EndGame";
        public static readonly EndGame Instance = new();
    }
}
