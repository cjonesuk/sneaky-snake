namespace AnECS;

public static class ComponentTypeInformation<T>
{
    public static readonly int Id = ComponentTypeRegistry.Register<T>();

    public static readonly string Name = typeof(T).FullName ?? typeof(T).Name;
}

public static class EntityTypeInformation<T1>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        ComponentTypeId type1 = ComponentTypeRegistry.GetComponentTypeId<T1>();

        EntityType = EntityType.Create([type1]);
    }
}

public static class EntityTypeInformation<T1, T2>
{
    public static readonly EntityType EntityType;

    static EntityTypeInformation()
    {
        ComponentTypeId type1 = ComponentTypeRegistry.GetComponentTypeId<T1>();
        ComponentTypeId type2 = ComponentTypeRegistry.GetComponentTypeId<T2>();

        EntityType = EntityType.Create([type1, type2]);
    }
}