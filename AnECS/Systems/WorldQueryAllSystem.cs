namespace AnECS;

internal sealed class WorldQueryAllSystem<T1> : IWorldSystem where T1 : struct
{
    private readonly EntityQueryAction<T1> _action;

    public WorldQueryAllSystem(EntityQueryAction<T1> action)
    {
        _action = action;
    }

    public void Execute(ref WorldSystemData data)
    {
        data.World.QueryEach(_action);
    }
}
