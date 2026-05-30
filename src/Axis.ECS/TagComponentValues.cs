namespace Axis.ECS;

/// <summary>Storage for tag components (zero instance fields). Holds no per-entity data; every operation is a no-op. Shared across all archetypes.</summary>
internal sealed class TagComponentValues : IComponentValues
{
    public static readonly TagComponentValues Shared = new();

    private TagComponentValues()
    {
    }

    public void AddDefault()
    {
    }

    public void Add<TInput>(ref TInput value) where TInput : unmanaged
    {
    }

    public void RemoveAndFillHoleAt(int index)
    {
    }

    public void Migrate(IComponentValues source, int sourceIndex)
    {
    }

    public void Clear()
    {
    }

    public unsafe void Write(int entityIndex, byte* data, int size)
    {
    }
}
