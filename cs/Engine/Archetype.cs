using System.Collections;
using System.Diagnostics;

namespace Engine;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal class Archetype
{
    private readonly ArchetypeSignature _signature;
    private readonly List<EntityId> _entityIds = new();
    private readonly Dictionary<Type, IList> _componentListByType = new();

    public Archetype(ArchetypeSignature signature)
    {
        _signature = signature;
        _componentListByType = CreateComponentListsByTypeDictionary(signature);
    }

    private static Dictionary<Type, IList> CreateComponentListsByTypeDictionary(ArchetypeSignature signature)
    {
        var result = new Dictionary<Type, IList>();

        foreach (var type in signature.ComponentTypes)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            var list = Activator.CreateInstance(listType) ?? throw new InvalidOperationException($"Could not create list of type {listType}");
            result[type] = (IList)list;
        }

        return result;
    }

    public void AddEntity(EntityId entityId, params object[] components)
    {
        var componentsByType = components.ToDictionary(c => c.GetType(), c => c);

        _entityIds.Add(entityId);

        foreach (var component in components)
        {
            var type = component.GetType();
            var list = _componentListByType[type];
            list.Add(component);
        }
    }

    public IReadOnlyList<T> GetComponents<T>()
    {
        Type type = typeof(T);
        var list = (List<T>)_componentListByType[type];
        return list;
    }

    private string GetDebuggerDisplay()
    {
        return $"Archetype: {_signature}, Entities: {_entityIds.Count}";
    }
}
