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
        var result = _engine.Entities.QueryAll<GridTransform, BasicShape>();

        int gridSize = 50;

        foreach (var (transforms, shapes) in result)
        {
            for (int index = 0; index < transforms.Count; ++index)
            {
                var shape = shapes[index];
                var transform = transforms[index];

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
}
