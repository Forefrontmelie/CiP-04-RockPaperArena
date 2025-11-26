namespace CiP_04_RockPaperArena.Domain.Dtos;

public class RoundDTO
{
    public int Round { get; init; }

    //public IList<MatchPair> Pairs { get; init; }
    public IList<PairDTO> Pairs { get; init; }

    public RoundDTO(int round, IList<PairDTO> pairs)
    {
        Round = round;
        Pairs = pairs;
    }
}