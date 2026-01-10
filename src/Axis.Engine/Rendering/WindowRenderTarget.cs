
using System.Numerics;
using Raylib_cs;

namespace Axis.Engine.Rendering;

internal struct ViewportInternal
{
    //public RenderPass? RenderPass;
}

internal sealed class WindowRenderTarget
{
    private Viewport[] _viewports;
    private ViewportInternal[] _viewportInternals;
    private Color _clearColor;

    public WindowRenderTarget(Color clearColor)
    {
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
        Raylib.BeginDrawing();

        int renderWidth = Raylib.GetRenderWidth();
        int renderHeight = Raylib.GetRenderHeight();

        Raylib.ClearBackground(_clearColor);

        var renderCommands = new RenderCommandQueue();
        var worldRenderer = new WorldRenderManager();

        for (int index = 0; index < _viewports.Length; index++)
        {
            ref var viewport = ref _viewports[index];
            ref var viewportInternal = ref _viewportInternals[index];

            var cameraView = worldRenderer.GenerateRenderCommands(viewport.Camera, renderCommands);

            int x = (int)(viewport.X * renderWidth);
            int y = (int)(viewport.Y * renderHeight);
            int width = (int)(viewport.Width * renderWidth);
            int height = (int)(viewport.Height * renderHeight);
            int centerX = x + (width / 2);
            int centerY = y + (height / 2);

            Vector2 viewportOffset = new Vector2(centerX, centerY);

            Camera2D camera2d = new Camera2D(viewportOffset, cameraView.Target, cameraView.Rotation, cameraView.Zoom);

            Raylib.BeginMode2D(camera2d);
            Raylib.BeginScissorMode(x, y, width, height);

            renderCommands.ApplyAndClear(new RenderContext());

            Raylib.EndScissorMode();
            Raylib.EndMode2D();
        }

        Raylib.EndDrawing();
    }
}
