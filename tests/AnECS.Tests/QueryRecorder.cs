namespace AnECS.Tests;

static class QueryRecorder
{
    public static List<(Id, T1)> Run<T1>(World world) where T1 : struct
    {
        var recorder = new List<(Id, T1)>();
        world.Query((ref Id id, ref T1 arg1) =>
        {
            recorder.Add((id, arg1));
        });
        return recorder;
    }

    public static List<(Id, T1, T2)> Run<T1, T2>(World world)
        where T1 : struct
        where T2 : struct
    {
        var recorder = new List<(Id, T1, T2)>();
        world.Query((ref Id id, ref T1 arg1, ref T2 arg2) =>
        {
            recorder.Add((id, arg1, arg2));
        });
        return recorder;
    }
}
