namespace CiP_04_RockPaperArena.Domain.Models;

public abstract class Participant
{

    public string Name { get; init; }
    public int Id { get; init; }

    public Participant(string name, int id)
    {
        Name = name;
        Id = id;
    }
}
