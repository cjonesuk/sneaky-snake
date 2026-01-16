using System.Numerics;

namespace PingPong.PlayGame;

internal struct Ball(float speed, Vector2 direction)
{
    public float Speed = speed;
    public Vector2 Direction = direction;
}