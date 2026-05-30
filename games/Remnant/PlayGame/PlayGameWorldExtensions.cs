using System.Numerics;
using Axis.ECS;
using Axis.Engine.Components;
using Raylib_cs;

namespace Remnant.PlayGame;

internal static class PlayGameWorldExtensions
{
    public static Entity SpawnRtsCamera(this IWorld world, RtsCameraController initialController)
    {
        var entity = world.SpawnEntity();
        entity.Set(new Transform3d(initialController.FocusPoint));
        entity.Set(new Camera3d(
            position: Vector3.Zero,
            target: initialController.FocusPoint,
            fovYDegrees: 60f,
            projection: Camera3dProjection.Perspective));
        entity.Set(initialController);
        return entity;
    }

    public static Entity SpawnFloor(this IWorld world, Vector3 position, Vector2 sizeXZ, Color color)
    {
        var entity = world.SpawnEntity();
        entity.Set(new Transform3d(position));
        entity.Set(new BasicShape3d(Shape3dKind.Plane, new Vector3(sizeXZ.X, 0f, sizeXZ.Y), color));
        return entity;
    }

    public static Entity SpawnCube(this IWorld world, Vector3 position, Vector3 size, Color color)
    {
        var entity = world.SpawnEntity();
        entity.Set(new Transform3d(position));
        entity.Set(new BasicShape3d(Shape3dKind.Cube, size, color));
        entity.Add<Selectable>();
        return entity;
    }
}
