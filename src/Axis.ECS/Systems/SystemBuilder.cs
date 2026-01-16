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

public ref struct SystemBuilder<T1, T2, T3>(IWorld World)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    public void ForEach(QueryEachEntityAction<WorldSystemContext, T1, T2, T3> action)
    {
        var system = new ForEachEntitySystem<T1, T2, T3>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3> action)
    {
        var system = new ForAllEntitiesSystem<T1, T2, T3>(action);
        World.Systems.AddSystem(system);
    }
}

public ref struct SystemBuilder<T1, T2, T3, T4>(IWorld World)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    public void ForEach(QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4> action)
    {
        var system = new ForEachEntitySystem<T1, T2, T3, T4>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4> action)
    {
        var system = new ForAllEntitiesSystem<T1, T2, T3, T4>(action);
        World.Systems.AddSystem(system);
    }
}

public ref struct SystemBuilder<T1, T2, T3, T4, T5>(IWorld World)
    where T1 : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    public void ForEach(QueryEachEntityAction<WorldSystemContext, T1, T2, T3, T4, T5> action)
    {
        var system = new ForEachEntitySystem<T1, T2, T3, T4, T5>(action);
        World.Systems.AddSystem(system);
    }

    public void ForAll(QueryAllEntitiesAction<WorldSystemContext, T1, T2, T3, T4, T5> action)
    {
        var system = new ForAllEntitiesSystem<T1, T2, T3, T4, T5>(action);
        World.Systems.AddSystem(system);
    }
}