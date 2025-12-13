using System.Diagnostics;

namespace Engine;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct EntityId
{
    public readonly long Id;

    public EntityId(int id, int version)
    {
        Id = ((long)version << 32) | (uint)id;
    }

    private string GetDebuggerDisplay()
    {
        return $"EntityId: {Id}";
    }
}
