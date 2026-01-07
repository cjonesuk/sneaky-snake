using System.Runtime.CompilerServices;

namespace AnECS;

unsafe static class DeferredAddEntityCommand
{
    private static readonly DeferredCommandAction ApplyAction = ApplyCreateEntity;

    public struct Payload
    {
        public Id Id;

        public Payload(Id id)
        {
            Id = id;
        }
    }

    public static (DeferredCommandAction, Payload) Make(ref Id id)
    {
        var payload = new Payload(id);
        return (ApplyAction, payload);
    }

    private static void ApplyCreateEntity(World world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.CreateEntityInternal(value.Id);
    }
}

