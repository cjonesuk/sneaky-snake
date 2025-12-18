using Engine;
using Raylib_cs;
using SneakySnake.Engine.Rendering;

namespace SneakySnake;

internal class BasicShapeRenderer : RenderQueue<BasicShapeRenderCommand>
{
    public override void Render()
    {
        for (int index = 0; index < _commands.Count; ++index)
        {
            var shape = _commands[index];

            switch (shape.Type)
            {
                case ShapeType.Rectangle:
                    Raylib.DrawRectangle((int)shape.X, (int)shape.Y, (int)shape.SizeX, (int)shape.SizeY, shape.Color);
                    break;

                case ShapeType.Circle:
                    Raylib.DrawCircle((int)shape.X, (int)shape.Y, shape.SizeX, shape.Color);
                    break;
            }
        }
    }
}