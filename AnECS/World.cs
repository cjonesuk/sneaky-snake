namespace AnECS;



public interface IWorld
{

}

internal sealed class World : IWorld
{
    private uint _nextId = 1;

    internal World()
    {

    }

    public static World Create()
    {
        return new World();
    }

    public Entity Entity()
    {
        Id id = new Id(this, _nextId++);
        return new Entity(id);
    }
}
