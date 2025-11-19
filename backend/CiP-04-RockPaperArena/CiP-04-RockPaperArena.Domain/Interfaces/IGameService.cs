using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IGameService
{
   
    // Return all pairs (matches) for round d (1..n-1)
    //RoundDTO GetPairsForSpecificRound(int d);

    // Return max number of rounds for n participants (n-1)
    int GetMaxNumberOfRounds(int n);

    // Return remaining unique pairs after D rounds have been played
    int GetRemainingUniquePairs(int d);

    // Return who player i meets in round d (0-based index)
    int GetOpponentIndex(int i, int d);

    // Return Participant by index
    public Participant GetParticipant(int index);

    // Return the Participant who is opponent of player i in round d
    public Participant GetOpponentParticipant(int i, int d);

    // Return full schedule for player i over rounds 1..n-1
    PlayerScheduleDTO GetPlayerSchedule(int i);


}

