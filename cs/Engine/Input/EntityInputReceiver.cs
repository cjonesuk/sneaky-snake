using Engine.Components;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;

namespace Engine.Input;

internal sealed class EntityInputReceiver : IInputReceiver
{
    private readonly IWorld _world;
    private readonly EntityId _entityId;

    public EntityInputReceiver(IWorld world, EntityId entityId)
    {
        _world = world;
        _entityId = entityId;
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        if (!_world.Entities.TryQueryById(_entityId, out var entity))
        {
            return;
        }

        entity.GetRef<InputActionReceiver>().PendingActions.Add(inputEvent.Action);
    }
}
