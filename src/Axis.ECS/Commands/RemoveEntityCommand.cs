using System.Runtime.CompilerServices;

namespace Axis.ECS.Commands;

unsafe static class RemoveEntityCommand
{
    private static readonly CommandAction ApplyAction = ApplyRemoveEntity;

    public struct Payload
    {
        public Id Id;

        public Payload(Id id)
        {
            Id = id;
        }
    }

    public static (CommandAction, Payload) Make(ref Id id)
    {
        var payload = new Payload(id);
        return (ApplyAction, payload);
    }

    private static void ApplyRemoveEntity(World world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.RemoveEntity(value.Id);
    }
}
