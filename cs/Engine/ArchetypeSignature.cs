using System.Diagnostics;

namespace Engine;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
readonly struct ArchetypeSignature : IEquatable<ArchetypeSignature>
{
    readonly Type[] _componentTypes;

    public ArchetypeSignature(IReadOnlyList<Type> componentTypes)
    {
        _componentTypes = componentTypes.OrderBy(t => t.FullName).ToArray();
    }

    public bool Equals(ArchetypeSignature other) => _componentTypes.SequenceEqual(other._componentTypes);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var type in _componentTypes)
            {
                hash = hash * 31 + type.GetHashCode();
            }
            return hash;
        }
    }

    private string GetDebuggerDisplay()
    {
        return string.Join("|", _componentTypes.Select(t => t.Name));
    }
}
