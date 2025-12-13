namespace Engine;

public interface IEntityComponentManager
{
    EntityId AddEntity(params object[] components);
    IReadOnlyList<TComponent> QueryAll<TComponent>();

    Tuple<IReadOnlyList<TComponent1>, IReadOnlyList<TComponent2>> QueryAll<TComponent1, TComponent2>();
}
