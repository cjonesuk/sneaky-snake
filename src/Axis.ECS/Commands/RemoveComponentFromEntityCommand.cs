using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class RemoveComponentFromEntityCommand
{
    public static void RemoveComponent<T>(this WorldCommandQueue queue, Id id)
        where T : unmanaged
    {
        var payload = new Applier<T>.Payload(id);
        queue.Write(ref payload, Applier<T>.Apply);
    }

    static class Applier<T> where T : unmanaged
    {
        public struct Payload(Id id)
        {
            public Id Id = id;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.RemoveComponentFromEntity<T>(value.Id);
        };
    }
}