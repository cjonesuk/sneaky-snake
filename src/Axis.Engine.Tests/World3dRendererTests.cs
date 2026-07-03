using System.Numerics;
using Axis.ECS;
using Axis.Engine.Components;
using Axis.Engine.Rendering;
using Axis.Engine.Resources;
using Raylib_cs;

namespace Axis.Engine.Tests;

public class World3dRendererTests
{
    [Fact]
    public void NoCamera_ReturnsRenderModeNone()
    {
        var world = World.Create();

        var renderer = World3dRenderer.Create(world, drawGrid: false);
        var context = NewContext();

        renderer.Apply(context);

        Assert.Equal(RenderType.None, context.RenderMode.RenderType);
        Assert.Equal(0, context.RenderCommands.Count);
    }

    [Fact]
    public void WithCameraAndDrawGridFalse_EmitsNoCommandsForEmptyWorld()
    {
        var world = World.Create();
        var camera = world.SpawnEntity();
        camera.Add<Camera3d>();

        var renderer = World3dRenderer.Create(world, drawGrid: false);
        renderer.SetCamera(camera);
        var context = NewContext();

        renderer.Apply(context);

        Assert.Equal(RenderType.Render3d, context.RenderMode.RenderType);
        Assert.Equal(0, context.RenderCommands.Count);
    }

    [Fact]
    public void WithDrawGridTrue_EmitsGridCommand()
    {
        var world = World.Create();
        var camera = world.SpawnEntity();
        camera.Add<Camera3d>();

        var renderer = World3dRenderer.Create(world, drawGrid: true);
        renderer.SetCamera(camera);
        var context = NewContext();

        renderer.Apply(context);

        Assert.Equal(1, context.RenderCommands.Count);
    }

    [Fact]
    public void EmitsOneCommandPerCubeEntity()
    {
        var world = World.Create();
        var camera = world.SpawnEntity();
        camera.Add<Camera3d>();

        SpawnCube(world, new Vector3(0, 0, 0));
        SpawnCube(world, new Vector3(10, 0, 0));
        SpawnCube(world, new Vector3(0, 0, 10));

        var renderer = World3dRenderer.Create(world, drawGrid: false);
        renderer.SetCamera(camera);
        var context = NewContext();

        renderer.Apply(context);

        Assert.Equal(3, context.RenderCommands.Count);
    }

    private static Entity SpawnCube(World world, Vector3 position)
    {
        var entity = world.SpawnEntity();
        entity.Set(new Transform3d(position));
        entity.Set(new BasicShape3d(Shape3dKind.Cube, Vector3.One, Color.Red));
        return entity;
    }

    private static RenderPrepContext NewContext()
    {
        return RenderPrepContext.Create(
            new Resolution(800, 600),
            new FrameResources(),
            new RenderCommandQueue());
    }
}
