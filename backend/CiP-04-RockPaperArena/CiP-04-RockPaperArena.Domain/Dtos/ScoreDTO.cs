using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Dtos;

public class ScoreDTO
{
    public string Name { get; init; }
    public int Id { get; init; }
    public int CurrentRound { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int Points { get; set; }

    public ScoreDTO(Score score)
    {
        Name = score.Name;
        Id = score.Id;
        CurrentRound = score.CurrentRound;
        Wins = score.Wins;
        Losses = score.Losses;
        Draws = score.Draws;
        Points = score.Points;
    }
}
