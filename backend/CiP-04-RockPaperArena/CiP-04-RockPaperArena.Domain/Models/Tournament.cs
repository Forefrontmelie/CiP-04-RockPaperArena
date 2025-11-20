using CiP_04_RockPaperArena.Domain.Dtos;

namespace CiP_04_RockPaperArena.Domain.Models;

public class Tournament
{
    public string PlayerName { get; init; }
    public IList<Participant> Participants { get; init; }
    public int CurrentRound { get; set; }
    public IList<RoundDTO> Rounds { get; set; }
    public bool IsActive { get; set; }

    public List<MatchResult> MatchResults { get; set; }

    public Tournament(string playerName, IList<Participant> participants)
    {
        PlayerName = playerName;
        Participants = participants;
        CurrentRound = 1;
        Rounds = new List<RoundDTO>();
        IsActive = true;
        MatchResults = new List<MatchResult>();
    }

    public int TotalRounds => Participants.Count - 1;
    public bool IsCompleted => CurrentRound > TotalRounds;
}