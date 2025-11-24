using CiP_04_RockPaperArena.Domain.Dtos;

namespace CiP_04_RockPaperArena.Domain.Models;

public class Tournament
{
    public IList<Participant> Participants { get; init; }
    public int CurrentRound { get; set; }
    public Dictionary<int, IList<Match>> RoundSchedule { get; set; }
    public bool IsActive { get; set; }
    public bool IsFinished { get; set; }

    public HashSet<int> ScoredRounds { get; set; } 
    public Scoreboard Scoreboard { get; set; }

    public int TotalRounds => Participants.Count - 1;
    public bool IsCompleted => CurrentRound > TotalRounds;

    public Tournament(string name, int playerId, IList<Participant> participants)
    {
        Participants = participants;
        CurrentRound = 1;
        RoundSchedule = new Dictionary<int, IList<Match>>();
        Scoreboard = new Scoreboard(Participants, CurrentRound);
        IsActive = true;
        IsFinished = false;
        ScoredRounds = new HashSet<int>();
    }


}