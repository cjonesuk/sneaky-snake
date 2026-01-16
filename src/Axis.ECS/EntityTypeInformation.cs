using System.Diagnostics;

namespace Axis.ECS;

public static class EntityTypeInformation
{
    public static readonly EntityType EntityType = EntityType.Empty;

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id
        ]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);

}

public static class EntityTypeInformation<T1, T2>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3, T4>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id,
            ComponentTypeInformation<T4>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3, T4, T5>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id,
            ComponentTypeInformation<T4>.Id,
            ComponentTypeInformation<T5>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3, T4, T5, T6>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id,
            ComponentTypeInformation<T4>.Id,
            ComponentTypeInformation<T5>.Id,
            ComponentTypeInformation<T6>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3, T4, T5, T6, T7>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id,
            ComponentTypeInformation<T4>.Id,
            ComponentTypeInformation<T5>.Id,
            ComponentTypeInformation<T6>.Id,
            ComponentTypeInformation<T7>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}

public static class EntityTypeInformation<T1, T2, T3, T4, T5, T6, T7, T8>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        EntityType = EntityType.Create([
            ComponentTypeInformation<T1>.Id,
            ComponentTypeInformation<T2>.Id,
            ComponentTypeInformation<T3>.Id,
            ComponentTypeInformation<T4>.Id,
            ComponentTypeInformation<T5>.Id,
            ComponentTypeInformation<T6>.Id,
            ComponentTypeInformation<T7>.Id,
            ComponentTypeInformation<T8>.Id]);
    }

    [Conditional("DEBUG")]
    internal static void DebugAssertSupports(EntityType entityType) => EntityType.DebugAssertSupports(entityType);
}