using System.Numerics;

namespace PingPong.PlayGame;

internal struct Paddle(float maxSpeed, float speed)
{
    public float MaxSpeed = maxSpeed;

    // Current speed of the paddle (positive = down, negative = up)
    public float Speed = speed;
}
