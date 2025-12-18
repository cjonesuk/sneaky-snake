using Engine;
using Engine.Rendering;

namespace SneakySnake;

internal class ExampleRenderQueueWorldTransformer : IRenderQueueWorldTransformer
{
    private readonly int _gridSize = 50;

    public void Generate(IWorld world, EntityId camera, IRenderPass renderPass)
    {
        var renderQueue = renderPass.GetQueue<BasicShapeRenderCommand>();

        var result = world.Entities.QueryAll<GridTransform, BasicShape>();

        foreach (var (transforms, shapes) in result)
        {
            for (int index = 0; index < transforms.Count; ++index)
            {
                var shape = shapes[index];
                var transform = transforms[index];

                switch (shape.Type)
                {
                    case ShapeType.Rectangle:
                        renderQueue.Enqueue(new BasicShapeRenderCommand(
                            transform.X * _gridSize,
                            transform.Y * _gridSize,
                            _gridSize,
                            _gridSize,
                            shape.Color,
                            shape.Type));
                        break;

                    case ShapeType.Circle:
                        renderQueue.Enqueue(new BasicShapeRenderCommand(
                            (transform.X * _gridSize) + (_gridSize / 2),
                            (transform.Y * _gridSize) + (_gridSize / 2),
                            _gridSize / 2.0f,
                            _gridSize / 2.0f,
                            shape.Color,
                            shape.Type));
                        break;
                }

            }
        }
    }
}