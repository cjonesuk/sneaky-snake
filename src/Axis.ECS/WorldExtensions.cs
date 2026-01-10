namespace Axis.ECS;

public static class WorldExtensions
{
    public static void AddSystem(this IWorld world, IWorldSystem system)
    {
        world.Systems.AddSystem(system);
    }

    public static SystemBuilder<T1> System<T1>(this IWorld world) where T1 : unmanaged
    {
        return new SystemBuilder<T1>(world);
    }

    public static SystemBuilder<T1, T2> System<T1, T2>(this IWorld world)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        return new SystemBuilder<T1, T2>(world);
    }
}