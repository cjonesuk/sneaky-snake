namespace Axis.ECS.Queries;

public enum QueryTermKind
{
    Include,
    Exclude,
}

public record struct QueryTerm(Id ComponentId, QueryTermKind Kind = QueryTermKind.Include);
