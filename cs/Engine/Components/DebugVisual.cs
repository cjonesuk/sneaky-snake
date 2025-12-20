using Raylib_cs;

namespace Engine.Components;

public struct DebugVisual
{
    public Color Color;

    public DebugVisual(Color color)
    {
        Color = color;
    }
}