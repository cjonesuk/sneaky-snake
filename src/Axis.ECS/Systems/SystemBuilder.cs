namespace Axis.ECS;

public ref struct SystemBuilder<T1>(IWorld World) where T1 : unmanaged
{
    public void ForEach(QueryEachEntityAction<WorldSystemContext, T1> action)
    {
        var system = new ForEachEntitySystem<T1>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<WorldSystemContext, T1> action)
    {
        var system = new ForAllEntitiesSystem<T1>(action);
        World.Systems.AddSystem(system);
    }
}

public ref struct SystemBuilder<T1, T2>(IWorld World)
    where T1 : unmanaged
    where T2 : unmanaged
{
    public void ForEach(QueryEachEntityAction<WorldSystemContext, T1, T2> action)
    {
        var system = new ForEachEntitySystem<T1, T2>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<WorldSystemContext, T1, T2> action)
    {
        var system = new ForAllEntitiesSystem<T1, T2>(action);
        World.Systems.AddSystem(system);
    }
}