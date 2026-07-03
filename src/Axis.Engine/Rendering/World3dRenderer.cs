using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;

namespace Axis.Engine.Rendering;

public sealed class World3dRenderer : IRenderer
{
    private readonly IWorld _world;
    private Entity _camera;
    private int _gridSlices;
    private float _gridSpacing;
    private bool _drawGrid;
    private Query<Transform3d, BasicShape3d> _shapeQuery;
    private Query<Transform3d, BasicShape3d, Outline> _outlineQuery;

    private World3dRenderer(IWorld world, Entity camera, int gridSlices, float gridSpacing, bool drawGrid)
    {
        _world = world;
        _camera = camera;
        _gridSlices = gridSlices;
        _gridSpacing = gridSpacing;
        _drawGrid = drawGrid;

        _shapeQuery = DefineQuery.For<Transform3d, BasicShape3d>(_world).Build();
        _outlineQuery = DefineQuery.For<Transform3d, BasicShape3d, Outline>(_world).Build();
    }

    public static World3dRenderer Create(IWorld world, int gridSlices = 100, float gridSpacing = 1.0f, bool drawGrid = true)
    {
        return new World3dRenderer(world, Entity.Invalid, gridSlices, gridSpacing, drawGrid);
    }

    public void SetCamera(Entity camera)
    {
        _camera = camera;
    }

    public void Apply(IRenderPrepContext context)
    {
        if (!_camera.IsValid())
        {
            return;
        }

        ref var camera3d = ref _camera.GetRef<Camera3d>();

        context.SetRenderMode(RenderMode.Create3d(in camera3d));

        RenderCommandQueue renderCommands = context.RenderCommands;

        if (_drawGrid)
        {
            renderCommands.AddDebugGrid(_gridSlices, _gridSpacing, 0);
        }

        _shapeQuery.ForEach((Entity entity, ref Transform3d transform, ref BasicShape3d shape) =>
        {
            switch (shape.Kind)
            {
                case Shape3dKind.Cube:
                    renderCommands.AddCube(transform.Position, shape.Size, shape.Color, 0);
                    return;

                case Shape3dKind.Plane:
                    renderCommands.AddPlane(transform.Position, new Vector2(shape.Size.X, shape.Size.Z), shape.Color, 0);
                    return;

                case Shape3dKind.Sphere:
                case Shape3dKind.Cylinder:
                    // Not implemented yet for the foundation; will land alongside the first concrete use case.
                    return;
            }
        });

        _outlineQuery.ForEach((Entity entity, ref Transform3d transform, ref BasicShape3d shape, ref Outline outline) =>
        {
            if (shape.Kind != Shape3dKind.Cube) return;
            renderCommands.AddCubeWires(transform.Position, shape.Size, outline.Color, 0);
        });
    }
}
