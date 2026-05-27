using System.Numerics;

namespace Remnant.PlayGame;

/// <summary>RTS-style camera rig: orbits a focus point on the ground at a fixed distance, pitch, and yaw.</summary>
public struct RtsCameraController
{
    public Vector3 FocusPoint;
    public float Distance;
    public float MinDistance;
    public float MaxDistance;
    public float PitchRadians;
    public float YawRadians;
    public float PanSpeed;
    public float ZoomSpeed;
}
