namespace Axis.ECS;

public static class WorldExtensions
{
    public static void AddSystem(this IWorld world, IWorldSystem system)
    {
        World conreteWorld = (World)world;
        conreteWorld.Systems.AddSystem(system);
    }

    public static void RemoveAllSystems(this IWorld world)
    {
        World conreteWorld = (World)world;
        conreteWorld.Systems.RemoveAllSystems();
    }

    /// <summary>Enumerate the children of an entity. Zero-allocation; use with foreach.</summary>
    public static ChildrenEnumerable GetChildren(this IWorld world, Id parentId)
    {
        var concrete = (World)world;
        return new ChildrenEnumerable(concrete, parentId);
    }

    /// <summary>Remove the entity and all its descendants. Depth-first; safe inside a deferred-commands scope.</summary>
    public static void RemoveEntityRecursive(this IWorld world, Id parentId)
    {
        Span<Id> stackBuffer = stackalloc Id[32];
        int count = 0;
        Id[]? heapOverflow = null;

        foreach (var child in world.GetChildren(parentId))
        {
            if (count < stackBuffer.Length)
            {
                stackBuffer[count] = child.Id;
            }
            else
            {
                heapOverflow ??= new Id[64];
                if (count - stackBuffer.Length >= heapOverflow.Length)
                {
                    Array.Resize(ref heapOverflow, heapOverflow.Length * 2);
                }
                heapOverflow[count - stackBuffer.Length] = child.Id;
            }
            count++;
        }

        int stackCount = Math.Min(count, stackBuffer.Length);
        for (int i = 0; i < stackCount; i++)
        {
            world.RemoveEntityRecursive(stackBuffer[i]);
        }
        if (heapOverflow is not null)
        {
            int overflowCount = count - stackBuffer.Length;
            for (int i = 0; i < overflowCount; i++)
            {
                world.RemoveEntityRecursive(heapOverflow[i]);
            }
        }

        world.RemoveEntity(parentId);
    }
}

/// <summary>Zero-allocation enumerable over entities whose <see cref="Parent"/> points at a given Id.</summary>
public readonly ref struct ChildrenEnumerable
{
    private readonly World _world;
    private readonly Id _parentId;

    internal ChildrenEnumerable(World world, Id parentId)
    {
        _world = world;
        _parentId = parentId;
    }

    public Enumerator GetEnumerator() => new(_world, _parentId);

    public ref struct Enumerator
    {
        private readonly World _world;
        private readonly Id _parentId;
        private readonly ReadOnlySpan<Archetype> _archetypes;
        private int _archetypeIndex;
        private int _entityIndex;
        private ReadOnlySpan<Id> _currentIds;
        private ReadOnlySpan<Parent> _currentParents;

        internal Enumerator(World world, Id parentId)
        {
            _world = world;
            _parentId = parentId;
            _archetypes = world.ParentQuery.Run();
            _archetypeIndex = -1;
            _entityIndex = -1;
            _currentIds = default;
            _currentParents = default;
        }

        public readonly Entity Current => Entity.Create(_world, _currentIds[_entityIndex]);

        public bool MoveNext()
        {
            while (true)
            {
                _entityIndex++;
                while (_entityIndex >= _currentIds.Length)
                {
                    _archetypeIndex++;
                    if (_archetypeIndex >= _archetypes.Length)
                    {
                        return false;
                    }
                    var archetype = _archetypes[_archetypeIndex];
                    _currentIds = archetype.GetEntityIds();
                    archetype.TryGetColumnSpan<Parent>(out var parents);
                    _currentParents = parents;
                    _entityIndex = 0;
                }
                if (_currentParents[_entityIndex].Value == _parentId)
                {
                    return true;
                }
            }
        }
    }
}
