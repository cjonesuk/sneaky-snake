namespace Axis.ECS;

internal sealed class EntityIdManager
{
    private const uint MaxComponents = 1 << 10; // 1024 components
    private const uint ComponentIdStart = 1;
    private const uint EntityIdStart = MaxComponents + ComponentIdStart;
    private uint _nextEntityId = EntityIdStart;
    private uint _nextComponentId = ComponentIdStart;

    public Id AllocateComponentId()
    {
        return Id.Make(_nextComponentId++, 0);
    }

    public Id AllocateEntityId()
    {
        return Id.Make(_nextEntityId++, 0);
    }
}
