using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetCurrentTournamentAsync();
    Task SaveTournamentAsync(Tournament tournament);
    Task ClearTournamentAsync();
    bool HasActiveTournament();
}
