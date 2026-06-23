using Axis.ECS;
using Remnant.PlayGame.Collision;
using Axis.ECS.Queries;
using Axis.Engine.Components;
using System.Runtime.InteropServices;

namespace Remnant.PlayGame.Systems;

public sealed class CollisionSyncSystem : IWorldSystem
{
    private readonly ICollisionWorld _collisionWorld;
    private readonly List<ColliderProxy> _colliders;
    private Query<Transform3d, Collider>? _query;

    public CollisionSyncSystem(ICollisionWorld collisionWorld)
    {
        _collisionWorld = collisionWorld;
        _colliders = new List<ColliderProxy>();
    }

    public void Execute(ref WorldSystemContext data)
    {
        // TODO: Dislike that the system gets the world on execute and the query is built on first execute.
        var world = data.World;
        _query ??= DefineQuery.For<Transform3d, Collider>(data.World).Build();

        _colliders.Clear();

        _query.Value.Iterate((ids, transforms, colliders) =>
        {
            for (int index = 0; index < ids.Length; index++)
            {
                var transform = transforms[index];
                var collider = colliders[index];

                // TODO: improve this API
                var entity = world.GetEntity(ids[index]);

                _colliders.Add(new ColliderProxy(entity, transform.Position.XZ(), collider.FootprintRadius));
            }
        });

        var colliders = CollectionsMarshal.AsSpan(_colliders);
        _collisionWorld.Build(colliders);
    }
}
