using System.Numerics;
using Axis.Engine.Components;
using Axis.Engine.Rendering;

namespace Axis.Engine.Tests;

public class Camera3dTests
{
    [Fact]
    public void RenderMode_Create3d_PreservesAllFields()
    {
        var camera = new Camera3d
        {
            Position = new Vector3(1, 2, 3),
            Target = new Vector3(4, 5, 6),
            Up = new Vector3(0, 1, 0),
            FovYDegrees = 60f,
            Projection = Camera3dProjection.Perspective,
        };

        var mode = RenderMode.Create3d(in camera);

        Assert.Equal(RenderType.Render3d, mode.RenderType);
        Assert.Equal(camera.Position, mode.Mode3d.Position);
        Assert.Equal(camera.Target, mode.Mode3d.Target);
        Assert.Equal(camera.Up, mode.Mode3d.Up);
        Assert.Equal(camera.FovYDegrees, mode.Mode3d.FovYDegrees);
        Assert.Equal(camera.Projection, mode.Mode3d.Projection);
    }

    [Fact]
    public void RenderMode_Create3d_OrthographicProjectionPreserved()
    {
        var camera = new Camera3d
        {
            Position = Vector3.Zero,
            Target = Vector3.UnitZ,
            Up = Vector3.UnitY,
            FovYDegrees = 10f,
            Projection = Camera3dProjection.Orthographic,
        };

        var mode = RenderMode.Create3d(in camera);

        Assert.Equal(Camera3dProjection.Orthographic, mode.Mode3d.Projection);
    }
}
