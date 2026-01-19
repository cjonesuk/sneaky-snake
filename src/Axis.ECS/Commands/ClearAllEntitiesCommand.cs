using Axis.Core.Collections;

namespace Axis.ECS.Commands;

internal static class ClearAllEntitiesCommand
{
    private static readonly WorldCommandQueue.CommandAction ApplyAction = ApplyClearAllEntities;

    public struct Payload
    {
    }

    public static void ClearAllEntities(this WorldCommandQueue queue)
    {
        var payload = new Payload();
        queue.Write(ref payload, ApplyAction);
    }

    private static void ApplyClearAllEntities(ref World world, CommandPayload payload)
    {
        world.RemoveAllEntities();
    }
}