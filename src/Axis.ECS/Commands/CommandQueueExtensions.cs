namespace Axis.ECS.Commands;

internal static class CommandQueueExtensions
{
    public static void AddEntity(this CommandQueue queue, Id id)
    {
        var (action, payload) = AddEntityCommand.Make(ref id);

        queue.Write(ref payload, action);
    }

    public static void AddEntity<T1>(this CommandQueue queue, Id id, T1 c1) where T1 : unmanaged
    {
        var (action, payload) = AddEntityCommand<T1>.Make(ref id, ref c1);

        queue.Write(ref payload, action);
    }

    public static void AddEntity<T1, T2>(this CommandQueue queue, ref Id id, ref T1 c1, ref T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var (action, payload) = AddEntityCommand<T1, T2>.Make(ref id, ref c1, ref c2);

        queue.Write(ref payload, action);
    }

    public static void RemoveEntity(this CommandQueue queue, Id id)
    {
        var (action, payload) = RemoveEntityCommand.Make(ref id);

        queue.Write(ref payload, action);
    }

    public static void SetComponent<T>(this CommandQueue queue, ref Id id, ref T component) where T : unmanaged
    {
        var (action, payload) = SetComponentCommand<T>.Make(ref id, ref component);

        queue.Write(ref payload, action);
    }

    public static void AddComponent<T>(this CommandQueue queue, Id id) where T : unmanaged
    {
        var (action, payload) = AddComponentCommand<T>.Make(ref id);

        queue.Write(ref payload, action);
    }

    public static void ClearAllEntities(this CommandQueue queue)
    {
        var (action, payload) = ClearAllEntitiesCommand.Make();

        queue.Write(ref payload, action);
    }

}