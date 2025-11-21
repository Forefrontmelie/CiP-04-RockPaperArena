using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IGameService
{

    public Dictionary<int, IList<Match>> PlayHumanMatchCurrentRound(Dictionary<int, IList<Match>> RoundSchedule, int currentRound, Move move);
    public Dictionary<int, IList<Match>> PlayAiMatchesCurrentRound(Dictionary<int, IList<Match>> RoundSchedule, int currentRound);
    public Dictionary<int, IList<Match>> PlayAllAiMatches(Dictionary<int, IList<Match>> RoundSchedule);


    /// Plays a single Rock Paper Scissors game
    GameResult PlayGame(Move player1Move, Move player2Move);
       
    /// Generates random move for AI players   
    Move GenerateRandomMove();
      
    /// Plays a complete match between two participants   
    Match PlayMatch(Match match, int roundNumber, Move? humanMove);




}
