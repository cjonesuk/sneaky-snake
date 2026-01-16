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

    public static void AddEntity<T1, T2, T3>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        var payload = new Apply3<T1, T2, T3>.Payload(id, c1, c2, c3);
        queue.Write(ref payload, Apply3<T1, T2, T3>.Apply);
    }

    public static void AddEntity<T1, T2, T3, T4>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        var payload = new Apply4<T1, T2, T3, T4>.Payload(id, c1, c2, c3, c4);
        queue.Write(ref payload, Apply4<T1, T2, T3, T4>.Apply);
    }

    public static void AddEntity<T1, T2, T3, T4, T5>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        var payload = new Apply5<T1, T2, T3, T4, T5>.Payload(id, c1, c2, c3, c4, c5);
        queue.Write(ref payload, Apply5<T1, T2, T3, T4, T5>.Apply);
    }

    public static void AddEntity<T1, T2, T3, T4, T5, T6>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
    {
        var payload = new Apply6<T1, T2, T3, T4, T5, T6>.Payload(id, c1, c2, c3, c4, c5, c6);
        queue.Write(ref payload, Apply6<T1, T2, T3, T4, T5, T6>.Apply);
    }

    public static void AddEntity<T1, T2, T3, T4, T5, T6, T7>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
    {
        var payload = new Apply7<T1, T2, T3, T4, T5, T6, T7>.Payload(id, c1, c2, c3, c4, c5, c6, c7);
        queue.Write(ref payload, Apply7<T1, T2, T3, T4, T5, T6, T7>.Apply);
    }

    public static void AddEntity<T1, T2, T3, T4, T5, T6, T7, T8>(this WorldCommandQueue queue, Id id, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4, ref T5 c5, ref T6 c6, ref T7 c7, ref T8 c8)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged
    {
        var payload = new Apply8<T1, T2, T3, T4, T5, T6, T7, T8>.Payload(id, c1, c2, c3, c4, c5, c6, c7, c8);
        queue.Write(ref payload, Apply8<T1, T2, T3, T4, T5, T6, T7, T8>.Apply);
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

    static class Apply3<T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3);
        };
    }

    static class Apply4<T1, T2, T3, T4>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3, T4 C4)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
            public T4 C4 = C4;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3, ref value.C4);
        };
    }

    static class Apply5<T1, T2, T3, T4, T5>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3, T4 C4, T5 C5)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
            public T4 C4 = C4;
            public T5 C5 = C5;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3, ref value.C4, ref value.C5);
        };
    }

    static class Apply6<T1, T2, T3, T4, T5, T6>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3, T4 C4, T5 C5, T6 C6)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
            public T4 C4 = C4;
            public T5 C5 = C5;
            public T6 C6 = C6;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3, ref value.C4, ref value.C5, ref value.C6);
        };
    }

    static class Apply7<T1, T2, T3, T4, T5, T6, T7>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3, T4 C4, T5 C5, T6 C6, T7 C7)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
            public T4 C4 = C4;
            public T5 C5 = C5;
            public T6 C6 = C6;
            public T7 C7 = C7;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3, ref value.C4, ref value.C5, ref value.C6, ref value.C7);
        };
    }

    static class Apply8<T1, T2, T3, T4, T5, T6, T7, T8>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
        where T6 : unmanaged
        where T7 : unmanaged
        where T8 : unmanaged
    {
        public struct Payload(Id Id, T1 C1, T2 C2, T3 C3, T4 C4, T5 C5, T6 C6, T7 C7, T8 C8)
        {
            public Id Id = Id;
            public T1 C1 = C1;
            public T2 C2 = C2;
            public T3 C3 = C3;
            public T4 C4 = C4;
            public T5 C5 = C5;
            public T6 C6 = C6;
            public T7 C7 = C7;
            public T8 C8 = C8;
        }

        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
        {
            ref Payload value = ref payload.GetRef<Payload>();
            world.CreateEntityWithId(ref value.Id, ref value.C1, ref value.C2, ref value.C3, ref value.C4, ref value.C5, ref value.C6, ref value.C7, ref value.C8);
        };
    }
}
