using System.Numerics;
using Axis.ECS;
using Axis.Engine.Collision;
using Axis.Engine.Components;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;

internal static class PlayGameWorldExtensions
{
    private const float BallSize = 40f;
    private const float PaddleWidth = 40f;
    private const float PaddleHeight = 200f;
    private const float PaddleSpeed = 500f;


    public static Entity SpawnBall(this IWorld world, Vector2 position, Vector2 direction, Color color)
    {
        var halfExtents = new Vector2(BallSize / 2, BallSize / 2);
        var normalizedDirection = Vector2.Normalize(direction);

        return ((World)world).DefineEntity()
            .With(new Transform2d(position))
            .With(new BasicShape(ShapeType.Rectangle, halfExtents, color))
            .With(new Ball(400f, normalizedDirection))
            .With(new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero))
            .Build();
    }

    public static Entity SpawnPaddle(this IWorld world, Player playerNumber, Vector2 position, Color color)
    {
        var halfExtents = new Vector2(PaddleWidth / 2, PaddleHeight / 2);

        return ((World)world).DefineEntity()
            .With(new Transform2d(position))
            .With(new BasicShape(ShapeType.Rectangle, halfExtents, color))
            .With(new PossessedByPlayer(playerNumber))
            .With(new Paddle(PaddleSpeed, 0))
            .With(new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero))
            .Build();
    }

    public static Entity SpawnGoal(this IWorld world, Player playerNumber, Vector2 position, Vector2 halfExtents)
    {
        return ((World)world).DefineEntity()
            .With(new Transform2d(position))
            .With(new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero))
            .With(new Goal(playerNumber))
            .With(new BasicShape(ShapeType.Rectangle, halfExtents, Color.Blue))
            .Build();
    }

    public static Entity SpawnWall(this IWorld world, Vector2 position, Vector2 halfExtents)
    {
        return ((World)world).DefineEntity()
            .With(new Transform2d(position))
            .With(new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero))
            .With(new BasicShape(ShapeType.Rectangle, halfExtents, Color.Gray))
            .With(new Wall())
            .Build();
    }
}
