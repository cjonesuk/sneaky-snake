using Axis.ECS.Queries;

namespace Axis.ECS;

public interface IWorldSystem
{
    void Execute(ref WorldSystemContext data);
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

    public void Execute(ref WorldSystemContext context)
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