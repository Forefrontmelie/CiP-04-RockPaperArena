using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;


namespace CiP_04_RockPaperArena.Infrastructure;

public class ParticipantRepository : IParticipantRepository

{
    public IList<Participant> Participants { get; private set; }
    public int NextId { get; private set; }

    // internal ConcurrentDictionary<int, string> Players = new();



    public ParticipantRepository()
    {
        Participants = new List<Participant>
    {
        new HumanParticipant("Alice", 1),
        new HumanParticipant("Bob", 2),
        new HumanParticipant("Charlie", 3),
        new HumanParticipant("Diana", 4),
        new HumanParticipant("Ethan", 5),
        new HumanParticipant("Fiona", 6)/*, 
        new HumanParticipant("George", 7),
        new HumanParticipant("Hannah", 8),
        new HumanParticipant("Isaac", 9),
        new HumanParticipant("Julia", 10),
        new HumanParticipant("Kevin", 11),
        new HumanParticipant("Laura", 12),
        new HumanParticipant("Michael", 13),
        new HumanParticipant("Nina", 14),
        new HumanParticipant("Oscar", 15),
        new HumanParticipant("Paula", 16),
        new HumanParticipant("Quentin", 17),
        new HumanParticipant("Rachel", 18),
        new HumanParticipant("Samuel", 19),
        new HumanParticipant("Tina", 20) */
    };

        // NextId should be one greater than the highest id present
        NextId = Participants.Any() ? Participants.Max(p => p.Id) + 1 : 1;


    }

    public void AddParticipant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));

        var participant = new HumanParticipant(name, NextId);
        Participants.Add(participant);                                          //TODO: Använd dependency injection och ev factory
        NextId++;


        Console.WriteLine($"Added participant: {participant.Name} with Id: {participant.Id}");
    }

    public void RemoveParticipant(int id)
    {
        var participant = Participants.FirstOrDefault(p => p.Id == id);
        if (participant == null)
            throw new ArgumentException("No Participant with that Id exist. ", nameof(id));

        Participants.Remove(participant);
    }

    public void RemoveParticipantByIndex(int index)
    {
        if (index >= 0 && index < Participants.Count)
        {
            Participants.RemoveAt(index);
        }
    }

    public IList<Participant> GetAllParticipants()
    {
        if (Participants.Count == 0)
            throw new InvalidOperationException("No participants available.");

        return Participants;
    }

    public Participant? GetParticipantById(int id)      // <<<<-------------------- ÄNDRA TILL INDEX! Antingen input eller konverta om från id till index.
    {

        if (id >= 0 && id < Participants.Count)
        {
            return Participants[id];
        }
        return null;
    }



}
