namespace Axis.ECS;

public static class ComponentTypeInformation<T>
{
    public static readonly ComponentTypeId Id;

    public static readonly string Name;

    /// <summary>True if T has no instance fields (auto-detected as a tag/marker component).</summary>
    public static readonly bool IsTag;

    static ComponentTypeInformation()
    {
        ComponentTypeRegistration registration = ComponentTypeRegistry.Register<T>();
        Id = registration.TypeId;
        Name = registration.Name;
        IsTag = registration.IsTag;
    }
}
