namespace SneakySnake.Game;

using System.Diagnostics;
using Engine;
using global::Engine;

public class GridMovementSystem : ISystem
{
    private IGameEngine? _engine;
    private float _accumulator = 0f;
    private const float MoveInterval = 1.0f; // Move every 1 second

    public void Attached(IGameEngine engine)
    {
        Debug.Assert(_engine == null, "System is already attached to an engine.");

        _engine = engine;
    }

    public void Detached()
    {
        _engine = null;
    }

    public void Update(float deltaTime)
    {
        Debug.Assert(_engine != null, "Engine should be attached before updating the system.");

        _accumulator += deltaTime;

        if (_accumulator < MoveInterval)
        {
            return;
        }

        _accumulator -= MoveInterval;

        // var archetypeList = _engine.Entities.QueryAll<GridTransform, GridAnimation>();

        // foreach (var (transforms, animations) in archetypeList)
        // {
        //     for (int index = 0; index < transforms.Count; index++)
        //     {
        //         var transform = transforms[index];

        //         transform.X += animations[index].SpeedX;
        //         transform.Y += animations[index].SpeedY;

        //         transforms[index] = transform;
        //     }
        // }
    }
}