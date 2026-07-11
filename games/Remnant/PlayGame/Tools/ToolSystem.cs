using Axis.ECS;
using Axis.Engine.Components;
using Raylib_cs;

namespace Remnant.PlayGame.Tools;

internal sealed class ToolSystem : IWorldSystem
{
    private readonly ToolContext _context;

    public ToolSystem(ToolContext context)
    {
        _context = context;
    }


    public void Execute(ref WorldSystemContext data)
    {
        // Handle selection logic here
        if (_context.MouseState.IsCapturedByUI)
        {
            return;
        }

        if (!_context.Camera.IsValid())
        {
            return;
        }

        ref Camera3d camera = ref _context.Camera.GetRef<Camera3d>();
        PointerRay pointerRay = PointerPicker.Compute(camera, _context.MouseState.Position);
        _context.SetPointerRay(pointerRay);

        if (_context.ActiveTool != _context.DefaultTool)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                _context.ResetToDefaultTool();
                return;
            }
        }

        _context.ActiveTool.Update(_context);
    }
}