namespace Axis.ECS;

internal sealed class EntityIdManager
{
    private const uint MaxComponents = 1 << 10; // 1024 components
    private const uint ComponentIdStart = 1;
    private const uint EntityIdStart = MaxComponents + ComponentIdStart;
    private const byte MaxGeneration = byte.MaxValue;

    private uint _nextEntityIndex = EntityIdStart;
    private uint _nextComponentId = ComponentIdStart;
    private readonly Queue<uint> _freeIndices = new();
    private byte[] _generations = new byte[1024];

    public Id AllocateComponentId()
    {
        return Id.Make(_nextComponentId++, 0);
    }

    public Id AllocateEntityId()
    {
        if (_freeIndices.TryDequeue(out uint reusedIndex))
        {
            // Generation was bumped at Free-time, so _generations[reusedIndex] is the value to use now.
            return Id.Make(reusedIndex, _generations[reusedIndex]);
        }

        uint index = _nextEntityIndex++;
        EnsureGenerationsCapacity(index);
        _generations[index] = 0;
        return Id.Make(index, 0);
    }

    public void Free(Id id)
    {
        uint index = IdSpace.EntityIndex(id.Value);
        if (index < EntityIdStart || index >= _nextEntityIndex)
        {
            return;
        }

        byte currentGen = _generations[index];
        if (currentGen == MaxGeneration)
        {
            // Bumping would wrap to 0 and collide with the original Id at this index. Retire the slot.
            return;
        }

        _generations[index] = (byte)(currentGen + 1);
        _freeIndices.Enqueue(index);
    }

    private void EnsureGenerationsCapacity(uint index)
    {
        if (index >= _generations.Length)
        {
            int newSize = Math.Max((int)index + 1, _generations.Length * 2);
            Array.Resize(ref _generations, newSize);
        }
    }
}
