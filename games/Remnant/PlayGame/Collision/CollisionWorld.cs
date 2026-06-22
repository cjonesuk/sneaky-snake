namespace Remnant.PlayGame.Collision;

public interface ICollisionWorld
{

}

public sealed class CollisionWorld : ICollisionWorld
{
    private readonly IBroadphase _broadphase;

    public CollisionWorld(IBroadphase broadphase)
    {
        _broadphase = broadphase;
    }

}