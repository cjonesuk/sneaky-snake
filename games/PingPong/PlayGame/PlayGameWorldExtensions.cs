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

        return world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Rectangle, halfExtents, color),
            new Ball(400f, normalizedDirection),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero));
    }

    public static Entity SpawnPaddle(this IWorld world, int playerNumber, Vector2 position, Color color)
    {
        var halfExtents = new Vector2(PaddleWidth / 2, PaddleHeight / 2);

        Entity paddle = world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Rectangle, halfExtents, color),
            new PossessedByPlayer(playerNumber),
            new Paddle(PaddleSpeed, 0),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero));

        return paddle;
    }

    public static Entity SpawnGoal(this IWorld world, int playerNumber, Vector2 position, Vector2 halfExtents)
    {

        Entity goal = world.CreateEntity(
            new Transform2d(position),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero),
            new Goal(playerNumber),
            new BasicShape(ShapeType.Rectangle, halfExtents, Color.Blue));

        return goal;
    }

    public static Entity SpawnWall(this IWorld world, Vector2 position, Vector2 halfExtents)
    {
        Entity wall = world.CreateEntity(
            new Transform2d(position),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero),
            new BasicShape(ShapeType.Rectangle, halfExtents, Color.Gray),
            new Wall());

        return wall;
    }
}
