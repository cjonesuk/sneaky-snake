namespace Axis.ECS;

internal sealed class EntityIdManager
{
    private const uint MaxComponents = 1 << 10; // 1024 components
    private const uint ComponentIdStart = 1;
    private const uint EntityIdStart = MaxComponents + ComponentIdStart;
    private const byte MaxGeneration = byte.MaxValue;

    private uint _nextEntityIndex = EntityIdStart;
    private uint _nextComponentId = ComponentIdStart;
    private readonly Queue<Id> _freeIds = new();

    public Id AllocateComponentId()
    {
        return Id.Make(_nextComponentId++, 0);
    }

    public Id AllocateEntityId()
    {
        while (_freeIds.TryDequeue(out Id freedId))
        {
            byte gen = freedId.Generation;
            if (gen < MaxGeneration)
            {
                return Id.Make(freedId.Index, (byte)(gen + 1));
            }
            // gen == 255: bumping would wrap and collide with the original Id at this index. Retire the slot.
        }

        return Id.Make(_nextEntityIndex++, 0);
    }

    public void Free(Id id)
    {
        uint index = id.Index;
        if (index < EntityIdStart || index >= _nextEntityIndex)
        {
            return;
        }
        _freeIds.Enqueue(id);
    }
}
