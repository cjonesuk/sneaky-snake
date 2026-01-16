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
}
