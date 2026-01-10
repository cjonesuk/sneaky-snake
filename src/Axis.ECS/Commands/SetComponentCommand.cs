using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class SetComponentCommand
{
    public static void SetComponent<T>(this WorldCommandQueue queue, ref Id entity, ref T component)
        where T : unmanaged
    {
        var payload = new Applier<T>.Payload(entity, component);
        queue.Write(ref payload, Applier<T>.Apply);
    }

    static class Applier<T> where T : unmanaged
    {
        public struct Payload(Id id, T component)
        {
            public Id Id = id;
            public T Component = component;
        }


        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.SetComponentOnEntity(value.Id, value.Component);
        };
    }
}
