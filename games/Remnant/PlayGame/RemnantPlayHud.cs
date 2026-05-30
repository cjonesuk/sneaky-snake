using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.UI;
using Raylib_cs;

namespace Remnant.PlayGame;

/// <summary>v1 placeholder HUD: a left selection panel and a bottom build bar with toggleable build buttons.</summary>
internal sealed class RemnantPlayHud
{
    private bool _buildMenuOpen;
    private Query? _selectedQuery;

    public void Build(UiContext ui, IWorld world)
    {
        ui.BeginPanel("selection", new Rectangle(0, 0, 220, 720));
        ui.Label("Selection", new Vector2(20, 20));

        _selectedQuery ??= DefineQuery.For(world).With<Selected>().Build();
        ui.Label($"Selected: {_selectedQuery.Value.Count()}", new Vector2(20, 50));

        ui.EndPanel();

        ui.BeginPanel("buildbar", new Rectangle(220, 640, 1060, 80));

        if (ui.Button("Toggle Build", new Rectangle(240, 660, 140, 40)))
        {
            _buildMenuOpen = !_buildMenuOpen;
        }

        if (_buildMenuOpen)
        {
            if (ui.Button("Farm", new Rectangle(400, 660, 100, 40)))
            {
                Console.WriteLine("Build: Farm clicked (stub)");
            }

            if (ui.Button("Barracks", new Rectangle(510, 660, 100, 40)))
            {
                Console.WriteLine("Build: Barracks clicked (stub)");
            }
        }

        ui.EndPanel();
    }
}
