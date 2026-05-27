using System.Runtime.CompilerServices;

namespace Axis.ECS;

public readonly struct Entity
{
    private readonly World _world;
    private readonly Id _id;

    public static readonly Entity Invalid = new Entity(null!, Id.Invalid);

    public Id Id => _id;

    public IWorld World => _world;

    private Entity(World world, Id id)
    {
        _world = world;
        _id = id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Entity Create(World world, Id id)
    {
        return new Entity(world, id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal World GetWorld() => _world;

    public bool IsValid()
    {
        return _world != null && _id.IsValid();
    }
}

public static class EntityWorldExtensions
{
    public static EntityType GetEntityType(this Entity entity)
    {
        World world = entity.GetWorld();
        return world.GetEntityType(entity.Id);
    }

    public static void Set<T>(this Entity entity, T component) where T : unmanaged
    {
        World world = entity.GetWorld();
        Id componentId = world.Components.GetId<T>();
        world.EnsureComponentOnEntity(entity.Id, componentId, ref component);
    }

    public static void SetPair<T>(this Entity entity, T component, Entity target) where T : unmanaged
    {
        World world = entity.GetWorld();
        world.SetPairOnEntity(entity.Id, target.Id, ref component);
    }

    public static void Add<T>(this Entity entity) where T : unmanaged
    {
        World world = entity.GetWorld();
        world.AddComponentToEntity<T>(entity.Id);
    }

    public static bool Has<T>(this Entity entity) where T : unmanaged
    {
        World world = entity.GetWorld();
        return world.EntityHasComponent<T>(entity.Id);
    }

    public static void Remove<T>(this Entity entity) where T : unmanaged
    {
        World world = entity.GetWorld();
        world.RemoveComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Get a mutable reference to the component T from the entity.
    /// </summary>
    public static ref T GetRef<T>(this Entity entity) where T : unmanaged
    {
        World world = entity.GetWorld();
        return ref world.GetComponentFromEntity<T>(entity.Id);
    }

    /// <summary>
    /// Gets a copy of the component T from the entity.
    /// </summary>
    public static T Get<T>(this Entity entity) where T : unmanaged
    {
        World world = entity.GetWorld();
        return world.GetComponentFromEntity<T>(entity.Id);
    }

    public static bool IsAlive(this ref Entity entity)
    {
        World world = entity.GetWorld();
        return world.IsEntityAlive(entity.Id);
    }

    public static void Delete(this Entity entity)
    {
        World world = entity.GetWorld();
        world.RemoveEntity(entity.Id);
    }

    public static ExportedEntity DebugExport(this Entity entity)
    {
        World world = entity.GetWorld();
        return ExportedEntity.Create(world, entity.Id);
    }
    public static void AddEvent<TEvent>(this Entity entity, ref TEvent @event)
    {
        World world = entity.GetWorld();
        world.Events.AddEvent(entity.Id, @event);
    }

    /// <summary>Set or replace the entity's parent.</summary>
    public static void SetParent(this Entity entity, Entity parent)
    {
        var p = new Parent(parent.Id);
        entity.Set(p);
    }

    /// <summary>Remove the entity's parent. No-op if not parented.</summary>
    public static void RemoveParent(this Entity entity)
    {
        entity.Remove<Parent>();
    }

    /// <summary>True if the entity has a Parent component (regardless of whether the parent is alive).</summary>
    public static bool HasParent(this Entity entity)
    {
        return entity.Has<Parent>();
    }

    /// <summary>Try to get the live parent entity. False if not parented or the parent is dead.</summary>
    public static bool TryGetParent(this Entity entity, out Entity parent)
    {
        if (!entity.Has<Parent>())
        {
            parent = Entity.Invalid;
            return false;
        }

        World world = entity.GetWorld();
        Id parentId = entity.GetRef<Parent>().Value;
        if (!world.IsEntityAlive(parentId))
        {
            parent = Entity.Invalid;
            return false;
        }

        parent = world.GetEntity(parentId);
        return true;
    }

    /// <summary>Get the live parent. Throws if not parented or parent is dead.</summary>
    public static Entity GetParent(this Entity entity)
    {
        if (!entity.TryGetParent(out var parent))
        {
            throw new InvalidOperationException($"Entity {entity.Id} has no live parent.");
        }
        return parent;
    }
}

