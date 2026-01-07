using System.Runtime.CompilerServices;

namespace Axis.ECS.Commands;

internal static unsafe class AddComponentCommand<T> where T : unmanaged
{
    private static readonly CommandAction ApplyAction = ApplyAddComponent;

    public readonly record struct Payload(Id Id);

    public static (CommandAction, Payload) Make(ref Id id)
    {
        var payload = new Payload(id);
        return (ApplyAction, payload);
    }

    private static void ApplyAddComponent(IWorld world, void* payload)
    {
        ref Payload value = ref Unsafe.AsRef<Payload>(payload);
        world.AddComponentToEntity<T>(value.Id);
    }
}
