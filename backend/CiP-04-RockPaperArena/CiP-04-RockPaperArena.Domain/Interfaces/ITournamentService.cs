using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface ITournamentService
{
    // Tournament management
    Task StartTournamentAsync(string name, int players);
    Task<Tournament?> GetCurrentTournamentAsync();
    Task<int> GetCurrentRoundNumberAsync();
    public bool HasActiveTournament { get; }
    //List<PairDTO>? GetCurrentRoundPairs();  
    Task<Match> PlayMoveAsync(int intMove);
    public Task PerformAiMatchesAsync();
    public Task AdvanceRoundAsync();

    public Task<StatusDTO> GetHumanPlayersCurrentGameStatusAsync();
    public Task<ScoreboardDTO> GetScoreboardAsync();
    public Task FinishTournamentAsync();


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

