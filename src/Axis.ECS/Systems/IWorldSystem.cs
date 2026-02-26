using Axis.ECS.Queries;

namespace Axis.ECS;

public interface IWorldSystem
{
    void Execute(WorldSystemContext data);
}


internal sealed class DelegatedWorldSystem : IWorldSystem
{
    internal delegate void ExecuteDelegate(WorldSystemContext context, Archetype archetype);

    private readonly ArchetypeQuery _query;
    private readonly ExecuteDelegate _execute;

    internal DelegatedWorldSystem(ArchetypeQuery query, ExecuteDelegate execute)
    {
        _query = query;
        _execute = execute;
    }

    public void Execute(WorldSystemContext context)
    {
        using var _ = context.World.BeginDeferringCommands();
        var archetypes = _query.Run();

        foreach (Archetype archetype in archetypes)
        {
            _execute(context, archetype);
        }
    }
}

public ref struct Field<T> where T : unmanaged
{
    private readonly Span<T> _span;
    private readonly int _index;

    internal Field(Span<T> span, int index)
    {
        _span = span;
        _index = index;
    }

    public ref T Value => ref _span[_index];
}

// public ref struct Iter
// {
//     private readonly Archetype _archetype;
//     private readonly ReadOnlySpan<Id> _entityIds;
//     private int _current;

//     internal Iter(Archetype archetype)
//     {
//         _archetype = archetype;
//         _entityIds = archetype.GetEntityIds();
//         _current = -1;
//     }

//     public int Current => _current;

//     public Id EntityId => _entityIds[_current];

//     public Span<T> GetColumn<T>() where T : unmanaged
//     {
//         _archetype.TryGetColumnSpan<T>(out var span);
//         return span;
//     }

//     public bool MoveNext()
//     {
//         _current++;
//         return _current < _entityIds.Length;
//     }
// }

internal sealed class SystemBuilder
{

}