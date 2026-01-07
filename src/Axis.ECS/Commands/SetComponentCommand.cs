using System.Runtime.CompilerServices;

namespace Axis.ECS.Commands;

internal static unsafe class SetComponentCommand<T> where T : unmanaged
{
    private static readonly CommandAction ApplyAction = ApplySetComponent;
    public readonly struct Payload
    {
        public readonly Id Entity;
        public readonly T Component;

        public Payload(Id entity, T component)
        {
            Entity = entity;
            Component = component;
        }
    }

    public static (CommandAction, Payload) Make(ref Id id, ref T component)
    {
        return (ApplyAction, new Payload(id, component));
    }

    private static void ApplySetComponent(IWorld world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.SetComponentOnEntity(value.Entity, value.Component);
    }
}

internal static unsafe class RemoveComponentFromEntityCommand<T> where T : unmanaged
{
    private static readonly CommandAction ApplyAction = ApplyRemoveComponentFromEntity;
    public readonly struct Payload
    {
        public readonly Id Entity;

        public Payload(Id entity)
        {
            Entity = entity;
        }
    }

    public static (CommandAction, Payload) Make(ref Id id)
    {
        return (ApplyAction, new Payload(id));
    }

    private static void ApplyRemoveComponentFromEntity(IWorld world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.RemoveComponentFromEntity<T>(value.Entity);
    }
}