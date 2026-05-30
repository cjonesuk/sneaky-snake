using Axis.ECS;
using Axis.Engine.Input;
using Axis.Engine.UI;

namespace Remnant.PlayGame;

/// <summary>Drives the IMGUI HUD each frame. Registered first so click reactions land before gameplay systems run.</summary>
internal sealed class RemnantUiSystem : IWorldSystem
{
    private readonly UiContext _ui;
    private readonly RemnantPlayHud _hud;
    private readonly MouseState _mouse;

    public RemnantUiSystem(UiContext ui, RemnantPlayHud hud, MouseState mouse)
    {
        _ui = ui;
        _hud = hud;
        _mouse = mouse;
    }

    public void Execute(ref WorldSystemContext context)
    {
        _ui.BeginFrame(_mouse);
        _hud.Build(_ui, context.World);
    }
}
