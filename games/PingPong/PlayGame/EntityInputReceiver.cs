using Axis.ECS;
using Axis.Engine.Input;

namespace PingPong.PlayGame;

internal sealed class EntityInputReceiver : IInputReceiver
{
    private Entity _entity;

    public EntityInputReceiver()
    {
        _entity = Entity.Invalid;
    }

    public EntityInputReceiver(Entity entity)
    {
        _entity = entity;
    }

    public void SetEntity(Entity entity)
    {
        // Note: This method allows changing the entity associated with this receiver.
        // Ensure that the new entity is valid and has the necessary components to handle input events.
        _entity = entity;
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        if (!_entity.IsValid())
        {
            return;
        }

        _entity.World.Events.AddEvent(_entity.Id, ref inputEvent);
    }
}
