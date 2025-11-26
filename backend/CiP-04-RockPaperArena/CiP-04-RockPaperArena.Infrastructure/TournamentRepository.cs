using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;


namespace CiP_04_RockPaperArena.Infrastructure;

public class TournamentRepository : ITournamentRepository   // Lägg till lock el.del för Thread-safety
{
    private static Tournament? _staticTournament; // Shared across all instances

    public Tournament? GetCurrentTournament()
    {
        return _staticTournament;
    }

    public void SaveTournament(Tournament tournament)
    {
        _staticTournament = tournament;
    }

    public void ClearTournament()
    {
        _staticTournament = null;
    }


    public bool HasActiveTournament()
    {
        return _staticTournament?.IsActive == true;
    }



}
