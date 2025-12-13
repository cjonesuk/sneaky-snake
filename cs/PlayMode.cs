namespace SneakySnake;

internal class PlayMode : IGameMode
{
    public void Enable()
    {
        Console.WriteLine("Starting Play Mode...");
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Play Mode...");
    }

    public void Update(float deltaTime)
    {
        // Update logic for gameplay
    }
}
