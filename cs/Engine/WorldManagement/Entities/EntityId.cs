using System.Diagnostics;

namespace Engine.WorldManagement.Entities;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct EntityId
{
    public readonly long Id;

    public static readonly EntityId Invalid = new EntityId(0, 0);

    public EntityId(int id, int version)
    {
        Id = ((long)version << 32) | (uint)id;
    }

    public EntityId NextId()
    {
        int id = (int)(Id & 0xFFFFFFFF);
        int version = (int)(Id >> 32);

        return new EntityId(id + 1, version);
    }

    public EntityId NextVersion()
    {
        int id = (int)(Id & 0xFFFFFFFF);
        int version = (int)(Id >> 32);

        return new EntityId(id, version + 1);
    }

    public static bool operator ==(EntityId left, EntityId right)
    {
        return left.Id == right.Id;
    }

    public static bool operator !=(EntityId left, EntityId right)
    {
        return left.Id != right.Id;
    }

    public static bool operator <(EntityId left, EntityId right)
    {
        return left.Id < right.Id;
    }

    public static bool operator >(EntityId left, EntityId right)
    {
        return left.Id > right.Id;
    }

    private string GetDebuggerDisplay()
    {
        return $"EntityId: {Id}";
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}
