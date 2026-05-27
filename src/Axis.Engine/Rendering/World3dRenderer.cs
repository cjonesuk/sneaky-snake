using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;

namespace Axis.Engine.Rendering;

public sealed class World3dRenderer : IRenderer
{
    private Entity _camera;
    private int _gridSlices;
    private float _gridSpacing;
    private bool _drawGrid;

    private World3dRenderer(Entity camera, int gridSlices, float gridSpacing, bool drawGrid)
    {
        _camera = camera;
        _gridSlices = gridSlices;
        _gridSpacing = gridSpacing;
        _drawGrid = drawGrid;
    }

    public static World3dRenderer Create(int gridSlices = 100, float gridSpacing = 1.0f, bool drawGrid = true)
    {
        return new World3dRenderer(Entity.Invalid, gridSlices, gridSpacing, drawGrid);
    }

    public void SetCamera(Entity camera)
    {
        _camera = camera;
    }

    public void GenerateRenderCommands(
        ref RenderContext context,
        out RenderMode renderMode)
    {
        if (!_camera.IsValid())
        {
            renderMode = RenderMode.None;
            return;
        }

        ref var camera3d = ref _camera.GetRef<Camera3d>();

        renderMode = RenderMode.Create3d(in camera3d);

        var renderContext = context;

        if (_drawGrid)
        {
            renderContext.RenderCommands.AddGrid(_gridSlices, _gridSpacing, 0);
        }

        DefineQuery.For<Transform3d, BasicShape3d>(_camera.World).Build()
            .ForEach((Entity entity, ref Transform3d transform, ref BasicShape3d shape) =>
            {
                switch (shape.Kind)
                {
                    case Shape3dKind.Cube:
                        renderContext.RenderCommands.AddCube(transform.Position, shape.Size, shape.Color, 0);
                        return;

                    case Shape3dKind.Plane:
                        renderContext.RenderCommands.AddPlane(transform.Position, new Vector2(shape.Size.X, shape.Size.Z), shape.Color, 0);
                        return;

                    case Shape3dKind.Sphere:
                    case Shape3dKind.Cylinder:
                        // Not implemented yet for the foundation; will land alongside the first concrete use case.
                        return;
                }
            });
    }
}
