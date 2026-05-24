namespace Axis.ECS;

public static class WorldExtensions
{
    public static void AddSystem(this IWorld world, IWorldSystem system)
    {
        World conreteWorld = (World)world;
        conreteWorld.Systems.AddSystem(system);
    }

    public static void RemoveAllSystems(this IWorld world)
    {
        World conreteWorld = (World)world;
        conreteWorld.Systems.RemoveAllSystems();
    }
}