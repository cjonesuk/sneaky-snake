
using System.Numerics;
using Axis.Engine.Resources;
using Raylib_cs;

namespace Axis.Engine.Rendering;

internal struct ViewportInternal
{
    //public RenderPass? RenderPass;
}

internal sealed class WindowRenderTarget
{
    private readonly RenderCommandQueue _renderCommands;
    private readonly FrameResources _frameResources;
    private Viewport[] _viewports;
    private ViewportInternal[] _viewportInternals;
    private Color _clearColor;

    public WindowRenderTarget(Color clearColor)
    {
        _renderCommands = new RenderCommandQueue();
        _frameResources = new FrameResources();
        _clearColor = clearColor;
        _viewports = [];
        _viewportInternals = [];
    }

    public void SetViewports(Viewport[] viewports)
    {
        if (viewports.Length == 0)
        {
            throw new ArgumentException("At least one viewport must be provided.", nameof(viewports));
        }

        // todo: Pooling for render passes
        Array.Resize(ref _viewports, viewports.Length);
        Array.Resize(ref _viewportInternals, viewports.Length);
        Array.Copy(viewports, _viewports, viewports.Length);
        Array.Clear(_viewportInternals, 0, _viewportInternals.Length);
    }

    public void Render()
    {
        _frameResources.Reset();

        Raylib.BeginDrawing();

        int renderWidth = Raylib.GetRenderWidth();
        int renderHeight = Raylib.GetRenderHeight();

        Raylib.ClearBackground(_clearColor);

        for (int index = 0; index < _viewports.Length; index++)
        {
            ref var viewport = ref _viewports[index];
            ref var viewportInternal = ref _viewportInternals[index];

            int x = (int)(viewport.X * renderWidth);
            int y = (int)(viewport.Y * renderHeight);
            int width = (int)(viewport.Width * renderWidth);
            int height = (int)(viewport.Height * renderHeight);
            var resolution = new Resolution(width, height);

            var prepContext = RenderPrepContext.Create(resolution, _frameResources, _renderCommands);
            viewport.Renderer.Apply(prepContext);

            RenderMode renderMode = prepContext.RenderMode;

            if (renderMode.RenderType == RenderType.None)
            {
                continue;
            }

            if (renderMode.RenderType == RenderType.Render2d)
            {
                int centerX = x + (width / 2);
                int centerY = y + (height / 2);

                Vector2 viewportOffset = new Vector2(centerX, centerY);

                Camera2D camera2d = new Camera2D(viewportOffset, renderMode.Mode2d.Target, renderMode.Mode2d.Rotation, renderMode.Mode2d.Zoom);

                Raylib.BeginMode2D(camera2d);
            }
            else if (renderMode.RenderType == RenderType.Render3d)
            {
                Camera3D camera3d = new Camera3D(
                    renderMode.Mode3d.Position,
                    renderMode.Mode3d.Target,
                    renderMode.Mode3d.Up,
                    renderMode.Mode3d.FovYDegrees,
                    renderMode.Mode3d.Projection == Axis.Engine.Components.Camera3dProjection.Orthographic
                        ? CameraProjection.Orthographic
                        : CameraProjection.Perspective);

                Raylib.BeginMode3D(camera3d);
            }

            Raylib.BeginScissorMode(x, y, width, height);

            RenderContext renderContext = new(resolution, _frameResources);
            _renderCommands.ApplyAndClear(ref renderContext);

            Raylib.EndScissorMode();

            if (renderMode.RenderType == RenderType.Render2d)
            {
                Raylib.EndMode2D();
            }
            else if (renderMode.RenderType == RenderType.Render3d)
            {
                Raylib.EndMode3D();
            }
        }

        Raylib.EndDrawing();
    }
}
