namespace Axis.ECS;

internal sealed class ForEachEntitySystem<T1> : IWorldSystem where T1 : unmanaged
{
    private readonly QueryEachEntityAction<WorldSystemContext, T1> _action;

    public ForEachEntitySystem(QueryEachEntityAction<WorldSystemContext, T1> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryEach(ref data, _action);
    }
}

internal sealed class ForEachEntitySystem<T1, T2> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
{
    private readonly QueryEachEntityAction<WorldSystemContext, T1, T2> _action;

    public ForEachEntitySystem(QueryEachEntityAction<WorldSystemContext, T1, T2> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext context)
    {
        context.World.QueryEach(ref context, _action);
    }
}
