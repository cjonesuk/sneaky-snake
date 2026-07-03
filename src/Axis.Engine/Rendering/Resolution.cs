using Axis.Core.Collections;

namespace Axis.Engine.Rendering;

public readonly struct Resolution(int width, int height)
{
    public readonly int Width = width;
    public readonly int Height = height;
}
