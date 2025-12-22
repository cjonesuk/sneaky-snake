using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Renderers;
using Engine.WorldManagement;
using Raylib_cs;

namespace SneakySnake;

internal class ExampleWorldRenderer : IWorldRenderer
{

    public void Generate(IWorld world, EntityId cameraId, IRenderPass renderPass)
    {
        if (!world.Entities.TryQueryById(cameraId, out var cameraEntity))
        {
            return;
        }

        ref var cameraTransform = ref cameraEntity.GetRef<Transform2d>();
        ref var camera2d = ref cameraEntity.GetRef<Camera2d>();

        var cameraView = new Camera2dRenderView(
            cameraTransform.Position,
            camera2d.Zoom,
            cameraTransform.Rotation);

        renderPass.SetCamera(cameraView);

        ProcessBasicShapes(world, renderPass);
        ProcessText2d(world, renderPass);
    }

    private void ProcessBasicShapes(IWorld world, IRenderPass renderPass)
    {
        var renderQueue = renderPass.GetQueue<BasicShapeRenderCommand>();

        var result = world.Entities.QueryAll<Transform2d, BasicShape>();

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
                            transform.Position,
                            new Vector2(50, 50),
                            transform.Rotation,
                            shape.Color,
                            shape.Type));
                        break;

                    case ShapeType.Circle:
                        renderQueue.Enqueue(new BasicShapeRenderCommand(
                            transform.Position,
                            new Vector2(50, 50),
                            transform.Rotation,
                            shape.Color,
                            shape.Type));
                        break;
                }

            }
        }
    }

    private void ProcessText2d(IWorld world, IRenderPass renderPass)
    {
        var renderQueue = renderPass.GetQueue<TextRenderCommand>();

        var font = Raylib.GetFontDefault();

        var result = world.Entities.QueryAll<Transform2d, Text2d>();

        foreach (var (transforms, textList) in result)
        {
            for (int index = 0; index < transforms.Count; index++)
            {
                var text2d = textList[index];
                var transform = transforms[index];

                Vector2 textSize = Raylib.MeasureTextEx(font, text2d.Text, text2d.FontSize, 1);

                Vector2 textPosition = text2d.Alignment switch
                {
                    TextAlignment.Left => transform.Position,
                    TextAlignment.Center => new Vector2(
                        transform.Position.X - (textSize.X / 2),
                        transform.Position.Y - (textSize.Y / 2)
                    ),
                    _ => new Vector2(0, 0)
                };

                renderQueue.Enqueue(new TextRenderCommand(
                    textPosition,
                    text2d.FontSize,
                    1,
                    text2d.Color,
                    text2d.Text));
            }
        }
    }
}