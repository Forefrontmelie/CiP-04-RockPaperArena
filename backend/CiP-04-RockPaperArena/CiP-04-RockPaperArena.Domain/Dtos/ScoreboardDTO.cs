using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Dtos;

public class ScoreboardDTO
{
    public Dictionary<int, ScoreDTO> scores { get; set; }


    public ScoreboardDTO(Scoreboard scoreboard)
    {
        scores = new Dictionary<int, ScoreDTO>();

        foreach (var id in scoreboard.scores.Keys)
        {
            var latestScore = scoreboard.scores[id].Last();
            var scoreDto = new ScoreDTO(latestScore);
            scores.Add(id, scoreDto);
        }
    }
}