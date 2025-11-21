using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Domain.Interfaces;

public interface IGameService
{  
    /// Plays a single Rock Paper Scissors game
    GameResult PlayGame(Move player1Move, Move player2Move);
       
    /// Generates random move for AI players   
    Move GenerateRandomMove();
      
    /// Plays a complete match between two participants   
    Match PlayMatch(Participant player1, Participant player2, int roundNumber, Move? humanMove = null);




}
