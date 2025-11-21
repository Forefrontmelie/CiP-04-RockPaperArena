namespace CiP_04_RockPaperArena.Domain.Models;

public class Match
{
    public Participant Player1 { get; init; }
    public Participant Player2 { get; init; }
    public bool IsComplete { get; set; }

    public int currentRound { get; set; }
    public int player1Wins { get; set; }
    public int player2Wins { get; set; }
    public int draw { get; set; }

    public string Opponent => Player2.Name;   //Computed property
    public string Player => Player1.Name;   //Computed property


    public Match(Participant player1, Participant player2)   // Roundnumber måste styras på ett annat sätt, utifrån TournamentService
    {
        Player1 = player1;
        Player2 = player2;
        IsComplete = false;

        currentRound = 1;
        player1Wins = 0;
        player2Wins = 0;
        draw = 0;
    }

}

