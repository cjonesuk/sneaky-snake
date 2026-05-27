namespace Axis.ECS;

/// <summary>An entity identified by Id together with its current storage location.</summary>
public record struct EntityRef(Id Id, Archetype Archetype, int Index)
{
    public static readonly EntityRef None = default;

    public readonly bool Exists => Id.IsValid();

    public readonly EntityLocation Location => new(Archetype, Index);
}
