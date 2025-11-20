namespace CiP_04_RockPaperArena.Domain.Models;

public class MatchResult
{
    public Participant Player1 { get; init; }
    public Participant Player2 { get; init; }
    public Move Player1Move { get; set; }
    public Move Player2Move { get; set; }
    public GameResult Result { get; set; }
    public int RoundNumber { get; init; }
    public bool IsComplete { get; set; }

    public MatchResult(Participant player1, Participant player2, int roundNumber)
    {
        Player1 = player1;
        Player2 = player2;
        RoundNumber = roundNumber;
        IsComplete = false;
    }

    // Helper properties
    public Participant? Winner    // Returnera string "Player1", "Player2" eller "Draw" i stället?
    {
        get
        {
            switch (Result)
            {
                case GameResult.P1:
                    return Player1;
                case GameResult.P2:
                    return Player2;
                case GameResult.Draw:
                    return null;
                default:
                    return null;
            }
        }
    }

    public bool IsDraw => Result == GameResult.Draw;
}