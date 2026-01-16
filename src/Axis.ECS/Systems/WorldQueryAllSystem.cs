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