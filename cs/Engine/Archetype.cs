using System.Diagnostics;

namespace Engine;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal class Archetype
{
    private readonly ArchetypeSignature _signature;
    private readonly List<ArchetypeRow> _rows = new();

    public Archetype(ArchetypeSignature signature)
    {
        _signature = signature;
    }

    public void AddEntity(EntityId entityId, params object[] components)
    {
        var componentsByType = components.ToDictionary(c => c.GetType(), c => c);
        var row = new ArchetypeRow(entityId, componentsByType);

        _rows.Add(row);
    }

    private string GetDebuggerDisplay()
    {
        return $"Archetype: {_signature}, Entities: {_rows.Count}";
    }
}
