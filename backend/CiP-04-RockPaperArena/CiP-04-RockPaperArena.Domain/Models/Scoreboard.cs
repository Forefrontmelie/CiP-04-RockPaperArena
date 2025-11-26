namespace CiP_04_RockPaperArena.Domain.Models;

public class Scoreboard
{
    public Dictionary<int, IList<Score>> scores { get; set; }

    public Scoreboard(IList<Participant> participants, int currentRound) 
    {
        scores = new Dictionary<int, IList<Score>>();

        foreach (var p in participants)
        {
            scores.Add(p.Id, new List<Score> { new Score(p.Name, p.Id, currentRound) });
        }
    }
    
   
}
