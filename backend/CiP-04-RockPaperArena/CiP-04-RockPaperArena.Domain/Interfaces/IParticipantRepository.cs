using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IParticipantRepository
{

    // IEnumerable<Participant> GetAll();

    IList<Participant> GetAllParticipants();
    void AddParticipant(string name);
    void RemoveParticipant(int id);
    Participant? GetParticipantById(int id);
}