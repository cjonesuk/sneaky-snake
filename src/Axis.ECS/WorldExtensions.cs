namespace Axis.ECS;

public static class WorldExtensions
{
    public static void AddSystem(this IWorld world, IWorldSystem system)
    {
        world.Systems.AddSystem(system);
    }

    public static void RemoveAllSystems(this IWorld world)
    {
        world.Systems.RemoveAllSystems();
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

    public static SystemBuilder<T1, T2, T3> System<T1, T2, T3>(this IWorld world)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        return new SystemBuilder<T1, T2, T3>(world);
    }

    public static SystemBuilder<T1, T2, T3, T4> System<T1, T2, T3, T4>(this IWorld world)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        return new SystemBuilder<T1, T2, T3, T4>(world);
    }

    public static SystemBuilder<T1, T2, T3, T4, T5> System<T1, T2, T3, T4, T5>(this IWorld world)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        return new SystemBuilder<T1, T2, T3, T4, T5>(world);
    }
}