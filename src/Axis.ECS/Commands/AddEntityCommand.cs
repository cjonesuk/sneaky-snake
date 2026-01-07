using System.Runtime.CompilerServices;

namespace Axis.ECS.Commands;

unsafe static class AddEntityCommand
{
    private static readonly CommandAction ApplyAction = ApplyCreateEntity;

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

    private static void ApplyCreateEntity(World world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.CreateEntityWithId(value.Id);
    }
}


unsafe static class AddEntityCommand<T1> where T1 : unmanaged
{
    private static readonly CommandAction ApplyAction = ApplyCreateEntity;

    public struct Payload
    {
        public Id Id;
        public T1 C1;

        public Payload(Id id, T1 c1)
        {
            Id = id;
            C1 = c1;
        }
    }

    public static (CommandAction, Payload) Make(ref Id id, ref T1 c1)
    {
        var payload = new Payload(id, c1);
        return (ApplyAction, payload);
    }

    private static void ApplyCreateEntity(World world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.CreateEntityWithId(ref value.Id, ref value.C1);
    }
}


unsafe static class AddEntityCommand<T1, T2>
    where T1 : unmanaged
    where T2 : unmanaged
{
    private static readonly CommandAction ApplyAction = ApplyCreateEntity;

    public struct Payload
    {
        public Id Id;
        public T1 C1;
        public T2 C2;

        public Payload(Id id, T1 c1, T2 c2)
        {
            Id = id;
            C1 = c1;
            C2 = c2;
        }
    }

    public static (CommandAction, Payload) Make(ref Id id, ref T1 c1, ref T2 c2)
    {
        var payload = new Payload(id, c1, c2);
        return (ApplyAction, payload);
    }

    private static void ApplyCreateEntity(World world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2);
    }
}
