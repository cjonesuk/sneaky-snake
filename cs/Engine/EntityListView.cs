namespace Engine;

public readonly struct EntityListView<T>
{
    private readonly List<T> _components;

    internal EntityListView(List<T> components)
    {
        _components = components;
    }

    public int Count => _components.Count;

    public T this[int index]
    {
        get
        {
            return _components[index];
        }
        set
        {
            _components[index] = value;
        }
    }
}
