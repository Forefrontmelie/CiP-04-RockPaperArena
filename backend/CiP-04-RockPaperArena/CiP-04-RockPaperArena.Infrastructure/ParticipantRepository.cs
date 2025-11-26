using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;


namespace CiP_04_RockPaperArena.Infrastructure;

public class ParticipantRepository : IParticipantRepository       // Lägg till lock el.del för Thread-safety

{
    public IList<Participant> Participants { get; private set; }
    public int NextId { get; private set; }

    // internal ConcurrentDictionary<int, string> Players = new();



    public ParticipantRepository()
    {
        Participants = new List<Participant>
    {
        new AIParticipant("Alice", 1),
        new AIParticipant("Bob", 2),
        new AIParticipant("Charlie", 3),
        new AIParticipant("Diana", 4),
        new AIParticipant("Ethan", 5),
        new AIParticipant("Fiona", 6), 
        new AIParticipant("George", 7),
        new AIParticipant("Hannah", 8),
        new AIParticipant("Isaac", 9),
        new AIParticipant("Julia", 10),
        new AIParticipant("Kevin", 11),
        new AIParticipant("Laura", 12),
        new AIParticipant("Michael", 13),
        new AIParticipant("Nina", 14),
        new AIParticipant("Oscar", 15),
        new AIParticipant("Paula", 16),
        new AIParticipant("Quentin", 17),
        new AIParticipant("Rachel", 18),
        new AIParticipant("Samuel", 19),
        new AIParticipant("Tina", 20),
        new AIParticipant("Unn", 21)
    };

        // NextId should be one greater than the highest id present
        NextId = Participants.Any() ? Participants.Max(p => p.Id) + 1 : 1;


    }

    public Task AddParticipantAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));

        var participant = new HumanParticipant(name, NextId);
        Participants.Add(participant);                                          //TODO: Använd dependency injection och ev factory
        NextId++;


        Console.WriteLine($"Added participant: {participant.Name} with Id: {participant.Id}");
        return Task.CompletedTask;
    }

    public Task RemoveParticipantAsync(int id)
    {
        var participant = Participants.FirstOrDefault(p => p.Id == id);
        if (participant == null)
            throw new ArgumentException("No Participant with that Id exist. ", nameof(id));

        Participants.Remove(participant);
        return Task.CompletedTask;
    }

    public void RemoveParticipantByIndex(int index)
    {
        if (index >= 0 && index < Participants.Count)
        {
            Participants.RemoveAt(index);
        }
    }

    public Task<IList<Participant>> GetAllParticipantsAsync()
    {
        if (Participants.Count == 0)
            throw new InvalidOperationException("No participants available.");

        return Task.FromResult(Participants);
    }

    public Task<Participant?> GetParticipantByIdAsync(int id)      // <<<<-------------------- ÄNDRA TILL INDEX! Antingen input eller konverta om från id till index.
    {

        if (id >= 0 && id < Participants.Count)
        {
            return Task.FromResult<Participant?>(Participants[id]);
        }
        return Task.FromResult<Participant?>(null);
    }



}
