using System.Numerics;
using Axis.ECS;
using Axis.Engine.Components;
using Engine.Components;
using Raylib_cs;

namespace Axis.Engine.Rendering;

internal sealed class WorldRenderManager
{
    public WorldRenderManager()
    {
    }

    public Camera2dRenderView GenerateRenderCommands(Entity camera, RenderCommandQueue renderCommands)
    {
        // Problems
        // 1. Need access to context within queries
        // 2. Need to define render passes
        // 3. Need to define render queues within passes 

        ref var cameraTransform = ref camera.GetRef<Transform2d>();
        ref var camera2d = ref camera.GetRef<Camera2d>();

        var cameraView = new Camera2dRenderView(
            cameraTransform.Position,
            camera2d.Zoom,
            cameraTransform.Rotation);

        var world = camera.World;

        world.QueryEach((ref Id id, ref Transform2d transform, ref BasicShape shape) =>
        {
            var position = transform.Position;
            var halfExtents = shape.HalfExtents;
            var rotation = transform.Rotation;
            var color = shape.Color;
            var type = shape.Type;

            switch (type)
            {
                case ShapeType.Circle:
                    renderCommands.AddCircle(ref position, halfExtents.X, color, 0);
                    return;

                case ShapeType.Rectangle:
                    Vector2 origin = halfExtents; // Center of the rectangle
                    Rectangle rect = new Rectangle(
                        position.X,
                        position.Y,
                        halfExtents.X * 2,
                        halfExtents.Y * 2);

                    renderCommands.AddRectangle(ref rect, ref origin, rotation, color, 0);
                    break;
            }
        });

        return cameraView;
    }
}
