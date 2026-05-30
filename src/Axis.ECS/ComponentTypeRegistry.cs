namespace Axis.ECS;

using System.Collections.Concurrent;
using System.Reflection;

public record class ComponentTypeRegistration(ComponentTypeId TypeId, Type ClrType, string Name, bool IsTag);

public static class ComponentTypeRegistry
{
    private static int _nextTypeId = 0;
    private static readonly ConcurrentDictionary<ComponentTypeId, ComponentTypeRegistration> _registrationsById = new();

    public static ComponentTypeRegistration Register<T>()
    {
        int typeId = Interlocked.Increment(ref _nextTypeId) - 1;

        ComponentTypeId componentTypeId = new(typeId);
        bool isTag = HasNoInstanceFields(typeof(T));
        ComponentTypeRegistration registration = new(componentTypeId, typeof(T), typeof(T).Name, isTag);

        _registrationsById[componentTypeId] = registration;

        return registration;
    }

    public static Type GetTypeById(int typeId)
    {
        return _registrationsById[new ComponentTypeId(typeId)].ClrType;
    }

    private static bool HasNoInstanceFields(Type type)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length == 0;
    }
}
