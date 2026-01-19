using System.Numerics;
using Axis.ECS;
using Axis.Engine.Collision;
using Axis.Engine.Components;
using Engine.Components;
using Raylib_cs;

namespace PingPong.PlayGame;

internal static class PlayGameWorldExtensions
{
    public static Entity SpawnBall(this IWorld world, Vector2 position, Color color)
    {
        var halfExtents = new Vector2(20f, 20f);

        return world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Rectangle, halfExtents, color),
            new Ball(400f, Vector2.Normalize(new Vector2(1, 0))),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero));
    }

    public static Entity SpawnPaddle(this IWorld world, int playerNumber, Vector2 position, Color color)
    {
        var halfExtents = new Vector2(20f, 100f);

        Entity paddle = world.CreateEntity(
            new Transform2d(position),
            new BasicShape(ShapeType.Rectangle, halfExtents, color),
            new PossessedByPlayer(playerNumber),
            new Paddle(500, 0),
            new CollisionBody(CollisionShape.Aabb, halfExtents, Vector2.Zero));

        return paddle;
    }

    public static Entity SpawnGoal(this IWorld world, int playerNumber, Vector2 position)
    {
        var halfExtents = new Vector2(10f, 300f);

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
