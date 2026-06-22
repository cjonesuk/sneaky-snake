using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Input;

namespace Remnant.PlayGame;

/// <summary>Reads input events on entities with an <see cref="RtsCameraController"/> and mutates the rig (FocusPoint / Distance).</summary>
public sealed class RtsCameraInputSystem : IWorldSystem
{
    private Query<RtsCameraController>? _query;

    public void Execute(ref WorldSystemContext context)
    {
        var world = context.World;
        float deltaTime = context.DeltaTime;

        _query ??= DefineQuery.For<RtsCameraController>(world).Build();

        _query.Value.ForEach((Entity entity, ref RtsCameraController controller) =>
        {
            if (!world.Events.TryGetEventStream<InputEvent>(entity.Id, out var inputStream))
            {
                return;
            }

            Vector2 panInputXZ = Vector2.Zero;
            float zoomInput = 0f;

            foreach (var inputEvent in inputStream.AsSpan())
            {
                if (inputEvent.Id == PlayGameActions.PanUp)
                {
                    panInputXZ.Y -= 1f;
                }
                else if (inputEvent.Id == PlayGameActions.PanDown)
                {
                    panInputXZ.Y += 1f;
                }
                else if (inputEvent.Id == PlayGameActions.PanLeft)
                {
                    panInputXZ.X -= 1f;
                }
                else if (inputEvent.Id == PlayGameActions.PanRight)
                {
                    panInputXZ.X += 1f;
                }
                else if (inputEvent.Id == PlayGameActions.ZoomIn)
                {
                    zoomInput -= inputEvent.Value;
                }
                else if (inputEvent.Id == PlayGameActions.ZoomOut)
                {
                    zoomInput += inputEvent.Value;
                }
            }

            if (panInputXZ != Vector2.Zero)
            {
                if (panInputXZ.LengthSquared() > 1f)
                {
                    panInputXZ = Vector2.Normalize(panInputXZ);
                }

                float cosYaw = MathF.Cos(controller.YawRadians);
                float sinYaw = MathF.Sin(controller.YawRadians);

                // Rotate pan input into world XZ space so "PanUp" moves away from the camera.
                float worldX = cosYaw * panInputXZ.Y - sinYaw * panInputXZ.X;
                float worldZ = sinYaw * panInputXZ.Y + cosYaw * panInputXZ.X;

                float magnitude = controller.PanSpeed * controller.Distance * deltaTime;
                controller.FocusPoint.X -= worldX * magnitude;
                controller.FocusPoint.Z += worldZ * magnitude;
            }

            if (zoomInput != 0f)
            {
                controller.Distance = Math.Clamp(
                    controller.Distance + zoomInput * controller.ZoomSpeed,
                    controller.MinDistance,
                    controller.MaxDistance);
            }
        });
    }
}
