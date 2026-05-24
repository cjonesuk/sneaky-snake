using System.Diagnostics;
using System.Runtime.CompilerServices;
using Axis.Core.Collections;
using Axis.Core.Memory;

namespace Axis.ECS.Commands;

internal readonly struct CreateEntityPayload
{
    public readonly Id EntityId;
    public readonly int ComponentCount;

    public CreateEntityPayload(Id entityId, int componentCount)
    {
        EntityId = entityId;
        ComponentCount = componentCount;
    }
}

internal readonly struct ComponentEntry
{
    public readonly Id ComponentId;
    public readonly int Size;

    public ComponentEntry(Id componentId, int size)
    {
        ComponentId = componentId;
        Size = size;
    }
}

public ref struct EntityBuilder
{
    private readonly WorldCommandQueue _queue;
    private readonly World _world;
    private readonly Id _entityId;
    private readonly int _payloadStart;
    private int _componentCount;
    private bool _built;

    internal EntityBuilder(
        World world,
        WorldCommandQueue queue,
        Id entityId)
    {
        _world = world;
        _queue = queue;
        _entityId = entityId;
        _payloadStart = queue.Payload.Used;
        _componentCount = 0;
        _built = false;

        queue.Payload.WritePacked(
            new CreateEntityPayload(entityId, 0));
    }

    public EntityBuilder With<T>(in T component)
        where T : unmanaged
    {
        ThrowIfBuilt();

        Id componentId = _world.Components.GetId<T>();

        int size = NativeType<T>.Size;

        _queue.Payload.WritePacked(
            new ComponentEntry(componentId, size));

        _queue.Payload.WritePacked(component);

        _componentCount++;
        return this;
    }

    public unsafe Entity Build()
    {
        ThrowIfBuilt();

        *(CreateEntityPayload*)
            (_queue.Payload.Ptr + _payloadStart) =
            new CreateEntityPayload(
                _entityId,
                _componentCount);

        _queue.Enqueue(
            ApplyCreateEntity.Apply,
            _payloadStart);

        var entity = Entity.Create(_world, _entityId);

        _world.FlushCommands();

        _built = true;

        return entity;
    }

    [Conditional("DEBUG")]
    private void ThrowIfBuilt()
    {
        if (_built)
        {
            throw new InvalidOperationException(
                "EntityBuilder has already been built.");
        }
    }

    internal static unsafe class ApplyCreateEntity
    {
        public static readonly WorldCommandQueue.CommandAction Apply = static (ref world, payload) =>
        {
            byte* p = (byte*)payload.Ptr;

            // Header
            ref var header = ref Unsafe.AsRef<CreateEntityPayload>(p);
            p += sizeof(CreateEntityPayload);

            Span<Id> componentIds = stackalloc Id[header.ComponentCount];

            // First pass: read component ids
            for (int i = 0; i < header.ComponentCount; i++)
            {
                ref var entry = ref Unsafe.AsRef<ComponentEntry>(p);
                p += sizeof(ComponentEntry);

                componentIds[i] = entry.ComponentId;
                p += entry.Size;
            }

            // Todo: investigate 0 allocation approach
            EntityType entityType = EntityType.Create(componentIds);

            EntityLocation location = world.AllocateEntity(entityType, header.EntityId);
            Archetype archetype = location.Archetype;

            // Second pass: copy component data
            p = (byte*)payload.Ptr + sizeof(CreateEntityPayload);

            for (int i = 0; i < header.ComponentCount; i++)
            {
                ref var entry = ref Unsafe.AsRef<ComponentEntry>(p);
                p += sizeof(ComponentEntry);

                archetype.WriteComponent(
                    location.Index,
                    entry.ComponentId,
                    p,
                    entry.Size);

                p += entry.Size;
            }
        };
    }
}