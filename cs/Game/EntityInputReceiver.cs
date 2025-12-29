using Engine.Input;
using Engine.WorldManagement;
using Engine.WorldManagement.Entities;

namespace SneakySnake;

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
        var entity = _world.Entities.QueryById(_entityId);
        entity.GetRef<InputActionReceiver>().PendingActions.Add(inputEvent.Action);
    }
}
