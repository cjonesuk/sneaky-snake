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