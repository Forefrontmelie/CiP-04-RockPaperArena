namespace CiP_04_RockPaperArena.Domain.Models;

public class Score
{
    public string Name { get; init; }
    public int Id { get; init; }
    public int CurrentRound { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }

    public int Points { get; set; }

   public Score (string name, int id, int currentRound)
    {
        Name = name;
        Id = id;
        CurrentRound = currentRound;

        Wins = 0;
        Losses = 0;
        Draws = 0;
        Points = 0;
    }
}