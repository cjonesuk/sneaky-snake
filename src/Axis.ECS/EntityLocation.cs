using System.Runtime.CompilerServices;

namespace Axis.ECS;

public record struct EntityLocation(Archetype Archetype, int Index)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Id GetEntityId() => Archetype.GetEntityIds()[Index];
}
