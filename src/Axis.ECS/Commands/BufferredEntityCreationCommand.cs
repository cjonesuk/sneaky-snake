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

        queue.Payload.WritePacked(
            new CreateEntityPayload(entityId, 0));
    }

    public void With<T>(in T component)
        where T : unmanaged
    {
        Id componentId = _world.Components.GetId<T>();

        int size = NativeType<T>.Size;

        _queue.Payload.WritePacked(
            new ComponentEntry(componentId, size));

        _queue.Payload.WritePacked(component);

        _componentCount++;
    }

    public unsafe void Build()
    {
        *(CreateEntityPayload*)
            (_queue.Payload.Ptr + _payloadStart) =
            new CreateEntityPayload(
                _entityId,
                _componentCount);

        _queue.Enqueue(
            ApplyCreateEntity.Apply,
            _payloadStart);
    }

    internal static unsafe class ApplyCreateEntity
    {
        public static readonly WorldCommandQueue.CommandAction Apply = (ref World world, CommandPayload payload) =>
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

             EntityType entityType = EntityType.Create(componentIds);

             Archetype archetype = world.Archetypes.GetOrCreate(entityType);

             EntityLocation location = archetype.AllocateEntity(in header.EntityId);

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