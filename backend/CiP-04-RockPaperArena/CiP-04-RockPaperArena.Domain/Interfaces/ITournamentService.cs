using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface ITournamentService
{
    // Tournament management
    void StartTournament(string name, int players);
    Tournament? GetCurrentTournament();
    public int GetCurrentRoundNumber();
    public bool HasActiveTournament { get; }
    //List<PairDTO>? GetCurrentRoundPairs();  
    Match PlayMove(int intMove);
    public void PerformAiMatches();
    public void AdvanceRound();

    public StatusDTO GetHumanPlayersCurrentGameStatus();
    public ScoreboardDTO GetScoreboard();
    public void FinishTournament();


    // Round and pairing methods
    RoundDTO GetPairsForSpecificRound(int d);
    IList<Participant> GetPairsForRound(int roundNbr);
    
    // Tournament calculations
    int GetMaxNumberOfRounds(int n);
    int GetRemainingUniquePairs(int d);

    // Participant methods
    int GetOpponentIndex(int i, int d);
    Participant GetParticipant(int index);
    Participant GetParticipantById(int id);
    Participant GetOpponentParticipant(int i, int d);
    
    // Utility methods
    PlayerScheduleDTO GetPlayerScheduleDTO(int i);
    int ConvertIdToIndex(int id);
    void UpdateTSParticipantsList();
    

 
}

