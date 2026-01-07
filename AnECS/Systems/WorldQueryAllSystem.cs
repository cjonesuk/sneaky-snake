namespace AnECS;

internal sealed class ForAllEntitiesSystem<T1> : IWorldSystem where T1 : unmanaged
{
    private readonly QueryAllEntitiesAction<T1> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<T1> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemData data)
    {
        data.World.QueryAll(_action);
    }
}

internal sealed class ForAllEntitiesSystem<T1, T2> : IWorldSystem
    where T1 : unmanaged
    where T2 : unmanaged

{
    private readonly QueryAllEntitiesAction<T1, T2> _action;

    public ForAllEntitiesSystem(QueryAllEntitiesAction<T1, T2> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemData data)
    {
        data.World.QueryAll(_action);
    }
}