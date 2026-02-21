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

internal sealed class SystemBuilder
{
    
}