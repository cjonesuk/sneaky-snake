namespace Axis.ECS.Commands;

unsafe static class ClearAllEntitiesCommand
{
    private static readonly CommandAction ApplyAction = ApplyClearAllEntities;

    public struct Payload
    {
    }

    public static (CommandAction, Payload) Make()
    {
        var payload = new Payload();
        return (ApplyAction, payload);
    }

    private static void ApplyClearAllEntities(World world, void* payload)
    {
        world.ClearAll();
    }
}