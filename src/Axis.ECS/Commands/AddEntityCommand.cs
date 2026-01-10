using Axis.Core.Collections;

namespace Axis.ECS.Commands;

static class AddEntityCommand
{
    public static void AddEntity(this WorldCommandQueue queue, Id id)
    {
        var payload = new Apply0.Payload(id);
        queue.Write(ref payload, Apply0.Apply);
    }

    public static void AddEntity<T1>(this WorldCommandQueue queue, Id id, ref T1 c1)
        where T1 : unmanaged
    {
        var payload = new Apply1<T1>.Payload(id, c1);
        queue.Write(ref payload, Apply1<T1>.Apply);
    }

    public static void AddEntity<T1, T2>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        var payload = new Apply2<T1, T2>.Payload(id, c1, c2);
        queue.Write(ref payload, Apply2<T1, T2>.Apply);
    }

    static class Apply0
    {
        public struct Payload(Id id)
        {
            public Id Id = id;
        }

        public static WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(value.Id);
        };
    }

    static class Apply1<T1> where T1 : unmanaged
    {
        public struct Payload(Id id, T1 c1)
        {
            public Id Id = id;
            public T1 C1 = c1;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1);
        };
    }

    static class Apply2<T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2);
        };
    }
}
