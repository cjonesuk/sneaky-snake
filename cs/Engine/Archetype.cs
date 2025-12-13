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

    public IReadOnlyList<T> GetComponents<T>()
    {
        var components = new List<T>();

        foreach (var row in _rows)
        {
            if (row.Components.TryGetValue(typeof(T), out var component))
            {
                components.Add((T)component);
            }
        }

        return components;
    }

    private string GetDebuggerDisplay()
    {
        return $"Archetype: {_signature}, Entities: {_rows.Count}";
    }
}
