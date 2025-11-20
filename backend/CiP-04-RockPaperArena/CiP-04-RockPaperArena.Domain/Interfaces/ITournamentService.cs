using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface ITournamentService
{
    // Tournament management
    void StartTournament(string name, int players);
    bool HasActiveTournament { get; }
    Tournament? GetCurrentTournament();
    RoundDTO? GetCurrentRound();
    List<PairDTO>? GetCurrentRoundPairs();
    void AdvanceTournament();
    

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
    PlayerScheduleDTO GetPlayerSchedule(int i);
    int ConvertIdToIndex(int id);
    void UpdateParticipantsList();
    
    // Optional printing methods (for debugging/console output)
    void PrintRound(int roundNumber);
    void PrintMessage(string message);
 
}

