namespace Axis.ECS;

public static class ComponentTypeInformation<T>
{
    public static readonly ComponentTypeId Id;

    public static readonly string Name;

    static ComponentTypeInformation()
    {
        ComponentTypeRegistration registration = ComponentTypeRegistry.Register<T>();
        Id = registration.TypeId;
        Name = registration.Name;
    }
}
