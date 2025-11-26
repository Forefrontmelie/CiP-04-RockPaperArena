namespace CiP_04_RockPaperArena.Domain.Dtos;

public class StatusDTO
{
    public string Player { get; set; }
    public string Opponent { get; set; }
    public int CurrentRound { get; set; }
    public int SubRound { get; set; }
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public int Draws { get; set; }
    public bool IsComplete { get; set; }
    public StatusDTO(string player, string opponent, int currentRound, int subRound, int player1Wins, int player2Wins, int draws, bool isComplete)
    {
        Player = player;
        Opponent = opponent;
        CurrentRound = currentRound;
        SubRound = subRound;
        Player1Wins = player1Wins;
        Player2Wins = player2Wins;
        Draws = draws;
        IsComplete = isComplete;
    }


}