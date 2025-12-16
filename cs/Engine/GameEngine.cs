using Raylib_cs;

namespace Engine;

public struct EntityQueryResult<TComponent>
{

}

public class GameEngine : IGameEngine
{
    private readonly Settings _settings;
    private readonly IEntityComponentManager _entities;
    private readonly List<ISystem> _systems = new();

    public GameEngine(Settings settings)
    {
        _settings = settings;
        _entities = new EntityComponentManager();
    }

    public Settings Settings => _settings;
    public IEntityComponentManager Entities => _entities;

    public void SetSystems(IReadOnlyList<ISystem> systems)
    {
        _systems.Clear();
        _systems.AddRange(systems);

        foreach (var system in _systems)
        {
            system.Attached(this);
        }
    }

    public void Run(IReadOnlyList<IGameEngineObserver> observers)
    {
        Raylib.InitWindow(_settings.ScreenWidth, _settings.ScreenHeight, _settings.Title);

        foreach (var observer in observers)
        {
            observer.OnEngineStart(this);
        }

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            _entities.ProcessPendingCommands();

            foreach (var observer in observers)
            {
                observer.Update(this, deltaTime);
            }

            Raylib.BeginDrawing();

            foreach (var system in _systems)
            {
                system.Update(deltaTime);
            }

            Raylib.EndDrawing();
        }

        foreach (var observer in observers)
        {
            observer.OnEngineStop(this);
        }

        Raylib.CloseWindow();
    }
}
