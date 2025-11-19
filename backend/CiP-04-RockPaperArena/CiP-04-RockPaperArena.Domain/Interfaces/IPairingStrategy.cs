using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IPairingStrategy
{
    IList<Participant> RotateParticipants(List<Participant> participants, int roundNbr);

    int GetOpponentIndex(int participantIndex, int n, int roundNbr);
}
