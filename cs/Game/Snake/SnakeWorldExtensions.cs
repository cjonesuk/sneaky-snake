using System.Numerics;
using Engine.Collision;
using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;
using Raylib_cs;

namespace SneakySnake;

internal static class SnakeWorldExtensions
{
    private static readonly float _spacing = 25f;
    private static readonly Vector2 _segmentHalfSize = new Vector2(10f, 10f);

    public static EntityId SpawnSnake(
        this IWorld world,
        Vector2 position)
    {
        Transform2d transform = new Transform2d(position, 0.0f);
        SnakeControl control = new SnakeControl(
            maxSpeed: 300f,
            acceleration: 200f,
            deceleration: 300f,
            maxTurnRate: 180f);

        InputActionReceiver inputReceiver = InputActionReceiver.Create();
        SnakeSegments segments = new SnakeSegments(_spacing);

        CollisionBody collisionBody = new CollisionBody(CollisionShape.Circle, _segmentHalfSize, Vector2.Zero);
        BasicShape basicShape = new BasicShape(ShapeType.Circle, _segmentHalfSize, Color.Green);

        return world.Entities.AddEntity(
            transform,
            control,
            inputReceiver,
            segments,
            collisionBody,
            basicShape);
    }
}
