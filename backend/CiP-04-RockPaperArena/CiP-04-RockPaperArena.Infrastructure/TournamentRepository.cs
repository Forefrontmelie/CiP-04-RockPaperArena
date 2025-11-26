using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;


namespace CiP_04_RockPaperArena.Infrastructure;

public class TournamentRepository : ITournamentRepository   // Lägg till lock el.del för Thread-safety
{
    private static Tournament? _staticTournament; // Shared across all instances

    public Task<Tournament?> GetCurrentTournamentAsync()
    {
        return Task.FromResult(_staticTournament);
    }

    public Task SaveTournamentAsync(Tournament tournament)
    {
        _staticTournament = tournament;
        return Task.CompletedTask;
    }

    public Task ClearTournamentAsync()
    {
        _staticTournament = null;
        return Task.CompletedTask;
    }


    public bool HasActiveTournament()
    {
        return _staticTournament?.IsActive == true;
    }



}
