using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;

namespace Remnant.PlayGame;

/// <summary>Applies the rig state in <see cref="RtsCameraController"/> to the entity's <see cref="Camera3d"/>. Pure transformation; any source (input, scripted, AI) can drive the controller.</summary>
public sealed class RtsCameraSystem : IWorldSystem
{
    private Query<RtsCameraController, Camera3d>? _query;

    public void Execute(ref WorldSystemContext context)
    {
        _query ??= DefineQuery.For<RtsCameraController, Camera3d>(context.World).Build();

        _query.Value.ForEach((Entity entity, ref RtsCameraController controller, ref Camera3d camera) =>
        {
            float cosYaw = MathF.Cos(controller.YawRadians);
            float sinYaw = MathF.Sin(controller.YawRadians);
            float cosPitch = MathF.Cos(controller.PitchRadians);
            float sinPitch = MathF.Sin(controller.PitchRadians);

            Vector3 offset = new Vector3(
                cosYaw * cosPitch * controller.Distance,
                sinPitch * controller.Distance,
                sinYaw * cosPitch * controller.Distance);

            camera.Target = controller.FocusPoint;
            camera.Position = controller.FocusPoint + offset;
        });
    }
}
