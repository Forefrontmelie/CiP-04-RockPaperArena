using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface ITournamentRepository
{
    Tournament? GetCurrentTournament();
    void SaveTournament(Tournament tournament);
    void ClearTournament();
    bool HasActiveTournament();
}
