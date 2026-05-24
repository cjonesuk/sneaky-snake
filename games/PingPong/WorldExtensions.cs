using System.Numerics;
using Axis.ECS;
using Engine.Components;
namespace PingPong;

internal static class WorldExtensions
{
    public static Entity SpawnCamera2d(this IWorld world, Vector2 position, float zoom)
    {
        return ((World)world).DefineEntity()
            .With(new Transform2d(position))
            .With(new Camera2d(zoom))
            .Build();
    }
}
