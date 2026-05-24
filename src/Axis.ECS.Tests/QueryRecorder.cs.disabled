namespace Axis.ECS.Tests;

static class QueryRecorder
{
    public static List<Id> QueryEach<TContext>(World world, ref TContext context)
    {
        var recorder = new List<Id>();
        world.QueryEach(ref context, (ref context, ref iter) =>
        {
            Id id = iter.Id;
            recorder.Add(id);
        });
        return recorder;
    }

    public static List<(Id, T1)> QueryEach<TContext, T1>(World world, ref TContext context) where T1 : unmanaged
    {
        var recorder = new List<(Id, T1)>();
        world.QueryEach<TContext, T1>(ref context, (ref context, ref iter, ref arg1) =>
        {
            Id id = iter.Id;
            recorder.Add((id, arg1));
        });
        return recorder;
    }

    public static List<(Id, T1, T2)> QueryEach<TContext, T1, T2>(World world, ref TContext context)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var recorder = new List<(Id, T1, T2)>();
        world.QueryEach<TContext, T1, T2>(ref context, (ref context, ref iter, ref arg1, ref arg2) =>
        {
            Id id = iter.Id;
            recorder.Add((id, arg1, arg2));
        });
        return recorder;
    }

    public static List<(Id, T1, T2, T3)> QueryEach<TContext, T1, T2, T3>(World world, ref TContext context)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        var recorder = new List<(Id, T1, T2, T3)>();
        world.QueryEach<TContext, T1, T2, T3>(ref context, (ref context, ref iter, ref arg1, ref arg2, ref arg3) =>
        {
            Id id = iter.Id;
            recorder.Add((id, arg1, arg2, arg3));
        });
        return recorder;
    }

    public static List<(Id, T1, T2, T3, T4)> QueryEach<TContext, T1, T2, T3, T4>(World world, ref TContext context)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        var recorder = new List<(Id, T1, T2, T3, T4)>();
        world.QueryEach<TContext, T1, T2, T3, T4>(ref context, (ref context, ref iter, ref arg1, ref arg2, ref arg3, ref arg4) =>
        {
            Id id = iter.Id;
            recorder.Add((id, arg1, arg2, arg3, arg4));
        });
        return recorder;
    }

    public static List<(Id, T1, T2, T3, T4, T5)> QueryEach<TContext, T1, T2, T3, T4, T5>(World world, ref TContext context)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        var recorder = new List<(Id, T1, T2, T3, T4, T5)>();
        world.QueryEach<TContext, T1, T2, T3, T4, T5>(ref context, (ref context, ref iter, ref arg1, ref arg2, ref arg3, ref arg4, ref arg5) =>
        {
            Id id = iter.Id;
            recorder.Add((id, arg1, arg2, arg3, arg4, arg5));
        });
        return recorder;
    }
}
