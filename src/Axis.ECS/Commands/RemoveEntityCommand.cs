using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class RemoveEntityCommand
{
    public static void RemoveEntity(this WorldCommandQueue queue, Id id)
    {
        var payload = new Payload(id);
        queue.Write(ref payload, ApplyAction);
    }

    private static readonly WorldCommandQueue.CommandAction ApplyAction = (ref World world, CommandPayload payload) =>
      {
          ref Payload value = ref payload.GetRef<Payload>();
          world.RemoveEntity(value.Id);
      };

    struct Payload(Id id)
    {
        public Id Id = id;
    }
}
