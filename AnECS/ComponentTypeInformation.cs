namespace AnECS;

public static class ComponentTypeRegistry
{
    private static int _nextTypeId = 0;
    private static readonly Dictionary<ComponentTypeId, Type> _typeById = new();

    public static int Register<T>()
    {
        int typeId = Interlocked.Increment(ref _nextTypeId) - 1;

        _typeById[new ComponentTypeId(typeId)] = typeof(T);

        return typeId;
    }

    public static Type GetTypeById(int typeId)
    {
        return _typeById[new ComponentTypeId(typeId)];
    }

    public static ComponentTypeId GetComponentTypeId<T>()
    {
        return new ComponentTypeId(ComponentTypeInformation<T>.Id);
    }
}

public static class ComponentTypeInformation<T>
{
    public static readonly int Id = ComponentTypeRegistry.Register<T>();

    public static readonly string Name = typeof(T).FullName ?? typeof(T).Name;
}
