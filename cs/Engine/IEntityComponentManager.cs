
namespace Engine;

public interface IEntityComponentManager
{
    EntityId AddEntity(params object[] components);

    void RemoveAllEntities();

    EntityQueryResult<T1, T2> QueryById<T1, T2>(EntityId entityId);

    EntityQueryAllResult<T1, T2> QueryAll<T1, T2>();

}
