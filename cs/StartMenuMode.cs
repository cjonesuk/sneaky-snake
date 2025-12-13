namespace SneakySnake;

internal class StartMenuMode : IGameMode
{
    private readonly IEngine _engine;

    public StartMenuMode(IEngine engine)
    {
        _engine = engine;
    }

    public void Enable()
    {
        Console.WriteLine("Starting Start Menu Mode...");

        ILayer[] layers = {
            new BackgroundLayer(_engine),
            new UiLayer(_engine),
        };

        _engine.SetLayers(layers);
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Start Menu Mode...");
        _engine.ClearLayers();
    }

    public void Update(float deltaTime)
    {
        // Update logic for start menu
    }
}
