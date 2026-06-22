using System.Collections.Generic;
using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;
using Axis.Engine.Input;
using Raylib_cs;

namespace Remnant.PlayGame;

/// <summary>RMB intersects the ray with the y=0 ground plane and writes a <see cref="MoveTarget"/> onto every <see cref="Selected"/> entity.</summary>
internal sealed class MoveCommandSystem : IWorldSystem
{
    private readonly MouseState _mouse;
    private readonly List<Entity> _toCommand = new();
    private Query<Camera3d>? _cameraQuery;
    private Query? _selectedQuery;

    public MoveCommandSystem(MouseState mouse)
    {
        _mouse = mouse;
    }

    public void Execute(ref WorldSystemContext context)
    {
        if (_mouse.IsCapturedByUI) return;
        if (!_mouse.WasPressed(MouseButton.Right)) return;

        var world = context.World;
        if (!TryGetCamera(world, out Camera3d cam)) return;

        var raylibCamera = new Camera3D(
            cam.Position,
            cam.Target,
            cam.Up,
            cam.FovYDegrees,
            cam.Projection == Camera3dProjection.Orthographic
                ? CameraProjection.Orthographic
                : CameraProjection.Perspective);
        var ray = Raylib.GetScreenToWorldRay(_mouse.Position, raylibCamera);

        if (ray.Direction.Y >= 0f) return;
        float t = -ray.Position.Y / ray.Direction.Y;
        if (t < 0f) return;
        Vector3 groundHit = ray.Position + ray.Direction * t;

        _toCommand.Clear();
        _selectedQuery ??= DefineQuery.For(world).With<Selected>().Build();
        _selectedQuery.Value.ForEach((Entity entity) =>
        {
            _toCommand.Add(entity);
        });

        foreach (var entity in _toCommand)
        {
            float currentY = entity.GetRef<Transform3d>().Position.Y;
            var target = new Vector3(groundHit.X, currentY, groundHit.Z);
            entity.Set(new MoveTarget(target));
        }
    }

    private bool TryGetCamera(IWorld world, out Camera3d camera)
    {
        Camera3d found = default;
        bool got = false;
        _cameraQuery ??= DefineQuery.For<Camera3d>(world).Build();
        _cameraQuery.Value.ForEach((Entity entity, ref Camera3d c) =>
        {
            if (got) return;
            found = c;
            got = true;
        });
        camera = found;
        return got;
    }
}
