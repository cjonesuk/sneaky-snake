using Axis.ECS;

namespace Axis.Engine.Input;

/// <summary>Forwards input events onto a specific entity's event stream.</summary>
public sealed class EntityInputReceiver : IInputReceiver
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
