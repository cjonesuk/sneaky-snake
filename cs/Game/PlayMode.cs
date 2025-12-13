using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class PlayMode : IGameMode
{
    private readonly IGame _game;
    private readonly IGameEngine _engine;

    public PlayMode(IGame game, IGameEngine engine)
    {
        _game = game;
        _engine = engine;
    }

    public void Enable()
    {
        Console.WriteLine("Starting Play Mode...");
        ISystem[] systems =
        {
            new LevelRenderSystem()
        };
        _engine.SetSystems(systems);

        SpawnFood(2, 3, Color.Orange);
        SpawnFood(5, 7, Color.Red);
        SpawnFood(10, 12, Color.Yellow);

        _engine.Entities.AddEntity(new Transform2d(400, 200), new Text2d(
            "Press Enter to End Game",
            24,
            Color.Black,
            TextAlignment.Center
        ));
    }

    public void Disable()
    {
        Console.WriteLine("Disabling Play Mode...");
        _engine.SetSystems(Array.Empty<ISystem>());
    }

    public void Update(float deltaTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Console.WriteLine("Enter key pressed, ending game...");
            _game.EndGame();
        }
    }

    private void SpawnFood(int x, int y, Color color)
    {
        _engine.Entities.AddEntity(new GridTransform(x, y), new BasicShape(ShapeType.Circle, color), new FoodTag());
    }
}
