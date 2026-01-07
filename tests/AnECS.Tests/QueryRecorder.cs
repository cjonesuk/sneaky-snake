namespace AnECS.Tests;

static class QueryRecorder
{
    public static List<Id> QueryEach(World world)
    {
        var recorder = new List<Id>();
        world.QueryEach((ref Id id) =>
        {
            recorder.Add(id);
        });
        return recorder;
    }

    public static List<(Id, T1)> QueryEach<T1>(World world) where T1 : unmanaged
    {
        var recorder = new List<(Id, T1)>();
        world.QueryEach((ref Id id, ref T1 arg1) =>
        {
            recorder.Add((id, arg1));
        });
        return recorder;
    }

    public static List<(Id, T1, T2)> QueryEach<T1, T2>(World world)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var recorder = new List<(Id, T1, T2)>();
        world.QueryEach((ref Id id, ref T1 arg1, ref T2 arg2) =>
        {
            recorder.Add((id, arg1, arg2));
        });
        return recorder;
    }
}
