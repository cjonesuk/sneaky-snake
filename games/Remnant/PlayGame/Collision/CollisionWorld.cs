namespace Remnant.PlayGame.Collision;

public interface ICollisionWorld
{
    Bounds Bounds { get; }

    void Build(in ReadOnlySpan<ColliderProxy> items);
}

public sealed class CollisionWorld : ICollisionWorld
{
    private readonly IBroadphase _broadphase;

    public CollisionWorld(IBroadphase broadphase)
    {
        _broadphase = broadphase;
    }

    public IBroadphase Broadphase => _broadphase;
    public Bounds Bounds => _broadphase.Bounds;

    public void Build(in ReadOnlySpan<ColliderProxy> items)
    {
        _broadphase.Build(items);
    }
}