using System.Runtime.CompilerServices;
using Axis.ECS.Commands;

namespace Axis.ECS;

internal static unsafe class DeferredSetComponent<T> where T : unmanaged
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

