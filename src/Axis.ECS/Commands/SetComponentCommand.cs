using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class SetComponentCommand
{
    public static void SetComponent<T>(this WorldCommandQueue queue, Id entity, Id componentId, ref T component)
        where T : unmanaged
    {
        var payload = new Applier<T>.Payload(entity, componentId, component);
        queue.Write(ref payload, Applier<T>.Apply);
    }

    static class Applier<T> where T : unmanaged
    {
        public struct Payload(Id id, Id componentId, T component)
        {
            public Id Id = id;
            public Id ComponentId = componentId;
            public T Component = component;
        }


        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.SetComponentOnEntity(value.Id, value.ComponentId, ref value.Component);
        };
    }
}
