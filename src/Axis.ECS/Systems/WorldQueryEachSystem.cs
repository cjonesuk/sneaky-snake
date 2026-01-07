namespace Axis.ECS;

internal sealed class ForEachEntitySystem<T1> : IWorldSystem where T1 : unmanaged
{
    private readonly QueryEachEntityAction<T1> _action;

    public ForEachEntitySystem(QueryEachEntityAction<T1> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemData data)
    {
        data.World.QueryEach(_action);
    }
}

internal sealed class ForEachEntitySystem<T1, T2> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged

{
    private readonly QueryEachEntityAction<T1, T2> _action;

    public ForEachEntitySystem(QueryEachEntityAction<T1, T2> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemData data)
    {
        data.World.QueryEach(_action);
    }
}
