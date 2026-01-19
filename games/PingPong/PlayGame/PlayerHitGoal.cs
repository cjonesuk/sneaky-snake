namespace PingPong.PlayGame;

/// <summary>
/// The player who's goal was hit
/// </summary>
internal readonly struct PlayerGoalHit(Player playerNumber)
{
    public readonly Player PlayerNumber = playerNumber;
}