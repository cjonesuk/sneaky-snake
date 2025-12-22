using System.Numerics;
using Engine;
using Raylib_cs;
using SneakySnake.Engine.Rendering;

namespace SneakySnake;

internal class BasicShapeRenderer : RenderQueueRenderer<BasicShapeRenderCommand>
{
    public override void RenderCommands()
    {
        for (int index = 0; index < _commands.Count; ++index)
        {
            var shape = _commands[index];

            switch (shape.Type)
            {
                case ShapeType.Rectangle:
                    Vector2 center = shape.Position;
                    Vector2 origin = new Vector2(shape.Size.X / 2, shape.Size.Y / 2);
                    Rectangle rect = new Rectangle(center.X - shape.Size.X / 2, center.Y - shape.Size.Y / 2, shape.Size.X, shape.Size.Y);
                    Raylib.DrawRectanglePro(rect, origin, shape.Rotation, shape.Color);
                    break;

                case ShapeType.Circle:
                    Raylib.DrawCircle((int)shape.Position.X, (int)shape.Position.Y, shape.Size.X, shape.Color);
                    break;
            }
        }
    }
}