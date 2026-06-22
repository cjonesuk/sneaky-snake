using System.Collections.Generic;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;
using Axis.Engine.Input;
using Raylib_cs;

namespace Remnant.PlayGame;

/// <summary>LMB ray-casts against <see cref="Selectable"/> cubes; single-select with empty-click deselect. Skipped while UI has capture.</summary>
internal sealed class SelectionSystem : IWorldSystem
{
    private static readonly Color OutlineColor = new(255, 230, 50, 255);

    private readonly MouseState _mouse;
    private readonly List<Entity> _toDeselect = new();
    private Query<Camera3d>? _cameraQuery;
    private Query<Transform3d, BasicShape3d>? _pickableQuery;
    private Query? _selectedQuery;

    public SelectionSystem(MouseState mouse)
    {
        _mouse = mouse;
    }

    public void Execute(ref WorldSystemContext context)
    {
        if (_mouse.IsCapturedByUI) return;
        if (!_mouse.WasPressed(MouseButton.Left)) return;

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

        Entity bestHit = Entity.Invalid;
        float bestDistance = float.MaxValue;

        _pickableQuery ??= DefineQuery.For<Transform3d, BasicShape3d>(world).With<Selectable>().Build();
        _pickableQuery.Value.ForEach((Entity entity, ref Transform3d transform, ref BasicShape3d shape) =>
        {
            var halfSize = shape.Size * 0.5f;
            var box = new BoundingBox(transform.Position - halfSize, transform.Position + halfSize);
            var collision = Raylib.GetRayCollisionBox(ray, box);
            if (collision.Hit && collision.Distance < bestDistance)
            {
                bestDistance = collision.Distance;
                bestHit = entity;
            }
        });

        _toDeselect.Clear();
        _selectedQuery ??= DefineQuery.For(world).With<Selected>().Build();
        _selectedQuery.Value.ForEach((Entity entity) =>
        {
            _toDeselect.Add(entity);
        });

        foreach (var entity in _toDeselect)
        {
            entity.Remove<Selected>();
            entity.Remove<Outline>();
        }

        if (bestHit.IsValid())
        {
            bestHit.Add<Selected>();
            bestHit.Set(new Outline(OutlineColor));
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
