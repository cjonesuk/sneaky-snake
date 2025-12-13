using Engine;
using Raylib_cs;

namespace SneakySnake;

internal class EntityLayer : ILayer
{
    private readonly IGameEngine _engine;

    public EntityLayer(IGameEngine engine)
    {
        _engine = engine;
    }

    public void Render()
    {
        var result = _engine.Entities.QueryAll<Engine.GridTransform, BasicShape>();

        int gridSize = 50;

        foreach (var (transform, shape) in result)
        {
            switch (shape.Type)
            {
                case ShapeType.Rectangle:
                    Raylib.DrawRectangle(transform.X * gridSize, transform.Y * gridSize, gridSize, gridSize, shape.Color);
                    break;

                case ShapeType.Circle:
                    Raylib.DrawCircle((transform.X * gridSize) + (gridSize / 2), (transform.Y * gridSize) + (gridSize / 2), gridSize / 2, shape.Color);
                    break;
            }

        }
    }
}
