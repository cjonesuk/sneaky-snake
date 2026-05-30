using Raylib_cs;

namespace Axis.Engine.Components;

/// <summary>Generic visual marker: renderers may draw an outline of the entity in this colour.</summary>
public struct Outline
{
    public Color Color;

    public Outline(Color color)
    {
        Color = color;
    }
}
