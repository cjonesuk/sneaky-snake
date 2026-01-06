namespace AnECS;

public ref struct SystemBuilder<T1>(IWorld World) where T1 : struct
{
    public void ForEach(QueryEachEntityAction<T1> action)
    {
        var system = new ForEachEntitySystem<T1>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<T1> action)
    {
        var system = new ForAllEntitiesSystem<T1>(action);
        World.Systems.AddSystem(system);
    }
}

public ref struct SystemBuilder<T1, T2>(IWorld World)
    where T1 : struct
    where T2 : struct
{
    public void ForEach(QueryEachEntityAction<T1, T2> action)
    {
        var system = new ForEachEntitySystem<T1, T2>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<T1, T2> action)
    {
        var system = new ForAllEntitiesSystem<T1, T2>(action);
        World.Systems.AddSystem(system);
    }
}