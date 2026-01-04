namespace AnECS;

public record class ComponentTypeRegistration(ComponentTypeId TypeId, Type ClrType, string Name);

public static class ComponentTypeRegistry
{
    private static int _nextTypeId = 0;
    private static readonly Dictionary<ComponentTypeId, ComponentTypeRegistration> _registrationsById = new();

    public static ComponentTypeRegistration Register<T>()
    {
        int typeId = Interlocked.Increment(ref _nextTypeId) - 1;

        ComponentTypeId componentTypeId = new(typeId);
        ComponentTypeRegistration registration = new(componentTypeId, typeof(T), typeof(T).Name);

        _registrationsById[componentTypeId] = registration;

        return registration;
    }

    public static Type GetTypeById(int typeId)
    {
        return _registrationsById[new ComponentTypeId(typeId)].ClrType;
    }
}
