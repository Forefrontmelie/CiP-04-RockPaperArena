namespace CiP_04_RockPaperArena.Domain.Models;

public class Match
{
    public Participant Player1 { get; init; }
    public Participant Player2 { get; init; }
    public Move Player1Move { get; set; }
    public Move Player2Move { get; set; }
    public GameResult Result { get; set; }
    private int CurrentRound { get; set; }
    public bool IsComplete { get; set; }

    public int player1Wins { get; set; }
    public int player2Wins { get; set; }
    public int draw { get; set; }

    public string Opponent => Player2.Name;   //Computed property
    public string Player => Player1.Name;   //Computed property


    public Match(Participant player1, Participant player2, int roundNumber)   // Roundnumber måste styras på ett annat sätt, utifrån TournamentService
    {
        Player1 = player1;
        Player2 = player2;
        CurrentRound = roundNumber;
        IsComplete = false;

        player1Wins = 0;
        player2Wins = 0;
        draw = 0;
    }

    // Helper properties
    public Participant? Winner    // Returnera string "Player1", "Player2" eller "Draw" i stället?  ---   LÄGG TILL CHECK OM EN SPELARE NÅTT 2 VINSTER!
    {
        get
        {
            switch (Result)
            {
                case GameResult.P1:
                    player1Wins++;
                    subRound++;
                    return Player1;
                case GameResult.P2:
                    player2Wins++;
                    subRound++;
                    return Player2;
                case GameResult.Draw:
                    draw++;
                    subRound++;
                    return null;
                default:
                    return null;
            }
        }
    }

    public bool IsDraw => Result == GameResult.Draw;
}