using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IParticipantRepository
{

    // IEnumerable<Participant> GetAll();

    Task<IList<Participant>> GetAllParticipantsAsync();
    Task AddParticipantAsync(string name);
    Task RemoveParticipantAsync(int id);
    Task<Participant?> GetParticipantByIdAsync(int id);
}