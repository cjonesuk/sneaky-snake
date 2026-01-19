using System.Numerics;
using Axis.ECS;
using Axis.Engine.Collision;
using Axis.Engine.Components;
using Engine.Components;
using PingPong.PlayGame;
using Raylib_cs;
namespace PingPong;

internal static class WorldExtensions
{
    public static Entity SpawnCamera2d(this IWorld world, Vector2 position, float zoom)
    {
        return world.CreateEntity(new Transform2d(position), new Camera2d(zoom));
    }

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
}
