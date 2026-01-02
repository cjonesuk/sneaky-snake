namespace AnECS;

public record struct ComponentTypeId(int Value) : IComparable<ComponentTypeId>
{
    public int CompareTo(ComponentTypeId other)
    {
        return Value.CompareTo(other.Value);
    }

    public static implicit operator int(ComponentTypeId componentTypeId) => componentTypeId.Value;
}
