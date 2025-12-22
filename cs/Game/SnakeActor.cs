using Engine.Input;
using Engine.WorldManagement;
using Engine.WorldManagement.Actors;
using Engine.WorldManagement.Entities;

namespace SneakySnake;

internal sealed class SnakeActor : IActor, IInputReceiver
{
    private readonly EntityId _headId;
    private readonly List<EntityId> _bodySegmentIds;
    private readonly IWorld _world;

    public SnakeActor(EntityId entityId, IWorld world)
    {
        _headId = entityId;
        _world = world;
        _bodySegmentIds = new List<EntityId>();
    }

    public void OnAttached(IWorld world)
    {
    }

    public void ReceiveInput(InputEvent inputEvent)
    {
        var entity = _world.Entities.QueryById(_headId);

        entity.GetRef<SnakeControl>().PendingActions.Add(inputEvent.Action);
    }

    public void Tick(IWorld world, float deltaTime)
    {

    }

    public void Tick(float deltaTime)
    {
        throw new NotImplementedException();
    }
}
