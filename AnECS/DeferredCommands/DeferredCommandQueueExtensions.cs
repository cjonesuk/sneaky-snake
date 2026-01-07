namespace AnECS;

internal static class DeferredCommandQueueExtensions
{
    public static void EnqueueAddEntity(this DeferredCommandQueue queue, Id id)
    {
        var (action, payload) = DeferredAddEntityCommand.Make(ref id);

        queue.Write(ref payload, action);
    }

    public static void EnqueueSetComponent<T>(this DeferredCommandQueue queue, ref Id id, ref T component) where T : unmanaged
    {
        var (action, payload) = DeferredSetComponent<T>.Make(ref id, ref component);

        queue.Write(ref payload, action);
    }
}