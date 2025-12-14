namespace Engine;

public readonly struct EntityAccessor<T>
{
    private readonly List<T> _components;
    private readonly int _index;

    internal EntityAccessor(List<T> components, int index)
    {
        _components = components;
        _index = index;
    }


    public T Value
    {
        get => _components[_index];
        set => _components[_index] = value;
    }
}