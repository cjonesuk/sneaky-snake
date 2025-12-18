
using System.Numerics;
using Raylib_cs;

namespace Engine.Rendering;

internal struct ViewportInternal
{
    public bool Initialized;
    public Camera2D Camera2D;
    public RenderPass? RenderPass;
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

        Array.Resize(ref _viewports, viewports.Length);
        Array.Resize(ref _viewportInternals, viewports.Length);
        Array.Copy(viewports, _viewports, viewports.Length);
        Array.Clear(_viewportInternals, 0, _viewportInternals.Length);
    }

    public void Render()
    {
        // Populate each viewports RenderPass from its world and camera
        for (int index = 0; index < _viewports.Length; index++)
        {
            ref var viewport = ref _viewports[index];
            ref var viewportInternal = ref _viewportInternals[index];

            if (viewportInternal.RenderPass == null)
            {
                viewportInternal.RenderPass = viewport.World.CreateRenderPass();
            }

            viewport.World.UpdateRenderPass(viewportInternal.RenderPass, viewport.Camera);
        }

        Raylib.BeginDrawing();

        int renderWidth = Raylib.GetRenderWidth();
        int renderHeight = Raylib.GetRenderHeight();

        Raylib.ClearBackground(_clearColor);

        for (int index = 0; index < _viewports.Length; index++)
        {
            ref var viewport = ref _viewports[index];
            ref var viewportInternal = ref _viewportInternals[index];

            if (!viewportInternal.Initialized)
            {
                viewportInternal.Camera2D = new Camera2D();
                viewportInternal.Initialized = true;
            }

            int x = (int)(viewport.X * renderWidth);
            int y = (int)(viewport.Y * renderHeight);
            int width = (int)(viewport.Width * renderWidth);
            int height = (int)(viewport.Height * renderHeight);
            float centerX = x + (width / 2.0f);
            float centerY = y + (height / 2.0f);

            viewportInternal.Camera2D.Offset = new Vector2(centerX, centerY);

            Raylib.BeginMode2D(viewportInternal.Camera2D);
            Raylib.BeginScissorMode(x, y, width, height);

            viewport.RenderPass.Render();

            Raylib.EndScissorMode();
            Raylib.EndMode2D();
        }

        Raylib.EndDrawing();
    }
}
