using System.Collections.Generic;
using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;

namespace Remnant.PlayGame;

/// <summary>Lerps <see cref="Transform3d.Position"/> toward <see cref="MoveTarget.Position"/> each frame. Removes <see cref="MoveTarget"/> on arrival.</summary>
internal sealed class MovementSystem : IWorldSystem
{
    private const float MoveSpeed = 8f;
    private const float ArriveThreshold = 0.1f;

    private readonly List<(Entity Entity, Vector3 At)> _arrived = new();
    private Query<Transform3d, MoveTarget>? _query;

    public void Execute(ref WorldSystemContext context)
    {
        var world = context.World;
        float dt = context.DeltaTime;

        _arrived.Clear();
        _query ??= DefineQuery.For<Transform3d, MoveTarget>(world).Build();

        _query.Value.ForEach((Entity entity, ref Transform3d transform, ref MoveTarget target) =>
        {
            Vector3 delta = target.Position - transform.Position;
            delta.Y = 0f;
            float distance = delta.Length();
            if (distance < ArriveThreshold)
            {
                transform.Position = target.Position;
                _arrived.Add((entity, target.Position));
                return;
            }

            float step = MathF.Min(MoveSpeed * dt, distance);
            Vector3 direction = delta / distance;
            transform.Position += direction * step;
        });

        foreach (var (entity, _) in _arrived)
        {
            entity.Remove<MoveTarget>();
        }
    }
}
