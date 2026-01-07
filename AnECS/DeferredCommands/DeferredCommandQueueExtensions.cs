namespace AnECS;

internal static class DeferredCommandQueueExtensions
{
    public static void EnqueueAddEntity(this DeferredCommandQueue queue, Id id)
    {
        var (action, payload) = DeferredAddEntityCommand.Make(ref id);

        queue.Write(ref payload, action);
    }

    public static void EnqueueAddEntity<T1>(this DeferredCommandQueue queue, Id id, T1 c1) where T1 : unmanaged
    {
        var (action, payload) = DeferredAddEntityCommand<T1>.Make(ref id, ref c1);

        queue.Write(ref payload, action);
    }

    public static void EnqueueAddEntity<T1, T2>(this DeferredCommandQueue queue, ref Id id, ref T1 c1, ref T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var (action, payload) = DeferredAddEntityCommand<T1, T2>.Make(ref id, ref c1, ref c2);

        queue.Write(ref payload, action);
    }

    public static void EnqueueRemoveEntity(this DeferredCommandQueue queue, Id id)
    {
        var (action, payload) = DeferredRemoveEntityCommand.Make(ref id);

        queue.Write(ref payload, action);
    }


    public static void EnqueueSetComponent<T>(this DeferredCommandQueue queue, ref Id id, ref T component) where T : unmanaged
    {
        var (action, payload) = DeferredSetComponent<T>.Make(ref id, ref component);

        queue.Write(ref payload, action);
    }

}