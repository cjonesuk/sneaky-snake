using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class AddComponentCommand
{
    struct Payload(Id id)
    {
        public Id Id = id;
    }

    public static void AddComponent<T>(this WorldCommandQueue queue, Id id)
        where T : unmanaged
    {
        var payload = new Payload(id);
        queue.Write(ref payload, Applier<T>.ApplyAddComponent);
    }

    static class Applier<T> where T : unmanaged
    {
        public static readonly WorldCommandQueue.CommandAction ApplyAddComponent = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.AddComponentToEntity<T>(value.Id);
        };
    }
}