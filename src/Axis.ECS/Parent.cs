namespace Axis.ECS;

/// <summary>Component pointing at another entity that owns this one in a parent-child hierarchy.</summary>
public struct Parent
{
    public Id Value;

    public Parent(Id parent)
    {
        Value = parent;
    }
}
