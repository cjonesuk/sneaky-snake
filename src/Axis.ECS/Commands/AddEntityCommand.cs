using Axis.Core.Collections;

namespace Axis.ECS.Commands;

static class AddEntityCommand
{
    public static void AddEntity(this WorldCommandQueue queue, Id id)
    {
        var payload = new ApplyAddEntity.Payload(id);
        queue.Write(ref payload, ApplyAddEntity.Apply);
    }

    static class ApplyAddEntity
    {
        public struct Payload(Id id)
        {
            public Id Id = id;
        }

        public static WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.SpawnEntityWithId(value.Id);
        };
    }
}
