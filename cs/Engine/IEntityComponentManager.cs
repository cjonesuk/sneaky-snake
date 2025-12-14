
namespace Engine;

public interface IEntityComponentManager
{
    EntityId AddEntity(params object[] components);

    void RemoveAllEntities();

    EntityQueryResult<T1, T2> QueryAll<T1, T2>();

}
