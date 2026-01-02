namespace AnECS;

public static class ComponentTypeInformation<T>
{
    public static readonly int Id = TypeIdGenerator.NextTypeId();

    public static readonly string Name = typeof(T).FullName ?? typeof(T).Name;
}
