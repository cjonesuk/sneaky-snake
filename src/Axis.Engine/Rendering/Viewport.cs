
namespace Axis.Engine.Rendering;

public readonly struct Viewport(
    float x,
    float y,
    float width,
    float height,
    IRenderer renderer)
{
    public readonly float X = x;
    public readonly float Y = y;
    public readonly float Width = width;
    public readonly float Height = height;
    public readonly IRenderer Renderer = renderer;

    public static Viewport Fullscreen(IRenderer renderer)
    {
        return new Viewport(0.0f, 0.0f, 1.0f, 1.0f, renderer);
    }

    public static Viewport[] SplitColumns(IRenderer rendererLeft, IRenderer rendererRight)
    {
        return
        [
            new Viewport(0.0f, 0.0f, 0.5f, 1.0f, rendererLeft),
            new Viewport(0.5f, 0.0f, 0.5f, 1.0f, rendererRight)
        ];
    }


}