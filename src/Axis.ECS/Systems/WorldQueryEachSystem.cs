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

internal sealed class ForEachEntitySystem<T1, T2, T3> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    private readonly QueryEachEntityAction<WorldSystemContext, T1, T2, T3> _action;

    public ForEachEntitySystem(QueryEachEntityAction<WorldSystemContext, T1, T2, T3> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext context)
    {
        context.World.QueryEach(ref context, _action);
    }
}

internal sealed class ForEachEntitySystem<T1, T2, T3, T4> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    private readonly QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4> _action;

    public ForEachEntitySystem(QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext context)
    {
        context.World.QueryEach(ref context, _action);
    }
}

internal sealed class ForEachEntitySystem<T1, T2, T3, T4, T5> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    private readonly QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4, T5> _action;

    public ForEachEntitySystem(QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4, T5> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext context)
    {
        context.World.QueryEach(ref context, _action);
    }
}
