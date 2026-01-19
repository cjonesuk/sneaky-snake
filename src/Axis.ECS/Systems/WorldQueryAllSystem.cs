namespace Axis.ECS;

internal sealed class ForAllEntitiesSystem<T1> : IWorldSystem where T1 : unmanaged
{
    private readonly QueryAllEntitiesAction<WorldSystemContext, T1> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<WorldSystemContext, T1> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryAll(ref data, _action);
    }
}

internal sealed class ForAllEntitiesSystem<T1, T2> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
{
    private readonly QueryAllEntitiesAction<WorldSystemContext, T1, T2> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<WorldSystemContext, T1, T2> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryAll(ref data, _action);
    }
}

internal sealed class ForAllEntitiesSystem<T1, T2, T3> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    private readonly QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryAll(ref data, _action);
    }
}

internal sealed class ForAllEntitiesSystem<T1, T2, T3, T4> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    private readonly QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryAll(ref data, _action);
    }
}

internal sealed class ForAllEntitiesSystem<T1, T2, T3, T4, T5> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    private readonly QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4, T5> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4, T5> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemContext data)
    {
        data.World.QueryAll(ref data, _action);
    }
}
