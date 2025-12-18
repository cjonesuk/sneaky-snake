namespace Engine;

internal class EntityComponentManager : IEntityComponentManager
{
    private readonly Dictionary<ArchetypeSignature, Archetype> _archetypes = new();
    private readonly Dictionary<EntityId, EntityLocation> _entityLocations = new();
    private readonly EntityCommandBuffer _commandBuffer = new();
    private int _nextEntityId = 1;

    private EntityId GetNextEntityId()
    {
        int id = _nextEntityId++;
        return new EntityId(id, 0);
    }

    private ArchetypeSignature GetArchetypeSignature(params object[] components)
    {
        Type[] componentTypes = components.Select(c => c.GetType()).ToArray();
        return new ArchetypeSignature(componentTypes);
    }

    private Archetype GetOrCreateArchetype(ArchetypeSignature signature)
    {
        if (!_archetypes.TryGetValue(signature, out var archetype))
        {
            archetype = new Archetype(signature);
            _archetypes[signature] = archetype;
        }

        return archetype;
    }

    public EntityId AddEntity(params object[] components)
    {
        EntityId entityId = GetNextEntityId();
        _commandBuffer.AddEntity(entityId, components);

        return entityId;
    }

    private EntityId AddEntity(EntityCreationRequest request)
    {
        Console.WriteLine($"Adding entity {request.EntityId}");
        ArchetypeSignature signature = GetArchetypeSignature(request.Components);

        Archetype archetype = GetOrCreateArchetype(signature);

        EntityLocation location = archetype.AddEntity(request.EntityId, request.Components);
        _entityLocations[request.EntityId] = location;

        return request.EntityId;
    }

    public void ProcessPendingCommands()
    {
        var (removals, creations) = _commandBuffer.GetPendingCommands();

        foreach (var request in removals)
        {
            RemoveEntity(request);
        }

        foreach (var request in creations)
        {
            AddEntity(request);
        }

        _commandBuffer.ClearPendingCommands();
    }

    public void RemoveEntity(EntityId entityId)
    {
        _commandBuffer.RemoveEntity(entityId);
    }

    private void RemoveEntity(EntityRemovalRequest request)
    {
        Console.WriteLine($"Removing entity {request.EntityId}: Not yet implemented");
    }

    public EntityQueryAllResult<T1, T2> QueryAll<T1, T2>()
    {
        var componentTypes = new Type[] { typeof(T1), typeof(T2) };

        var archetypes = _archetypes.Values
            .Where(x => x.Signature.ContainsAll(componentTypes))
            .ToList();

        return new EntityQueryAllResult<T1, T2>(archetypes);
    }

    public EntityQueryResult QueryById(EntityId entityId)
    {
        var location = _entityLocations[entityId];
        return new EntityQueryResult(location);
    }
}
