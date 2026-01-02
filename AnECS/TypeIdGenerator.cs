namespace AnECS;

internal static class TypeIdGenerator
{
    private static int _nextTypeId = 1;

    public static int NextTypeId()
    {
        return Interlocked.Increment(ref _nextTypeId) - 1;
    }
}
