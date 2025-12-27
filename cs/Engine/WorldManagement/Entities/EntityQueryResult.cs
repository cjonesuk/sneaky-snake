using System.Runtime.InteropServices;

namespace Engine.WorldManagement.Entities;


public readonly struct EntityQueryResult
{
    private readonly EntityLocation _location;

    internal EntityQueryResult(EntityLocation location)
    {
        _location = location;
    }

    public T1 GetCopy<T1>()
    {
        return _location.Archetype.GetComponents<T1>()[_location.Index];
    }

    public ref T GetRef<T>()
    {
        var components = _location.Archetype.GetComponents<T>();
        var span = CollectionsMarshal.AsSpan(components);
        return ref span[_location.Index];
    }
}