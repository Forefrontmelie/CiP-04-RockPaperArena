using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using System.Linq.Expressions;

namespace CiP_04_RockPaperArena.Application.Services;

public class GameService : IGameService
{

    public GameResult PlayGame(Move player1Move, Move player2Move)
    {        
        if (player1Move == player2Move)
            return GameResult.Draw;


        switch (player1Move)
        {
            case Move.Rock:
                if (player2Move == Move.Scissors)
                    return GameResult.P1;
                else
                    return GameResult.P2;

            case Move.Paper:
                if (player2Move == Move.Rock)
                    return GameResult.P1;
                else
                    return GameResult.P2;

            case Move.Scissors:
                if (player2Move == Move.Paper)
                    return GameResult.P1;
                else
                    return GameResult.P2;

            default:
                return GameResult.P2;
        }
    }

    
    /// Generates a random move for AI players
    /// <returns>Random Move (Rock, Paper, or Scissors)</returns>
    public Move GenerateRandomMove()
    {
        var random = new Random();
        var moveValues = Enum.GetValues<Move>();
        return moveValues[random.Next(moveValues.Length)];
    }


    public MatchResult PlayMatch(Participant player1, Participant player2, int roundNumber, Move? humanMove = null)
    {
        var matchResult = new MatchResult(player1, player2, roundNumber);

        // Determine moves for both players
        Move player1Move;
        Move player2Move;


        if (humanMove == null)
        {

            // Both players are AI
            player1Move = GenerateRandomMove();
            player2Move = GenerateRandomMove();
            var gameResult = PlayGame(player1Move, player2Move);

            matchResult.Player1Move = player1Move;
            matchResult.Player2Move = player2Move;
            matchResult.Result = gameResult;  // Updated property name
        }
        else
        {
            // Human vs AI match - determine which player is human
            if (player1 is HumanParticipant)
            {
                player1Move = humanMove.Value;
                player2Move = GenerateRandomMove();
            }
            else if (player2 is HumanParticipant)
            {
                player1Move = GenerateRandomMove();
                player2Move = humanMove.Value;
            }
            else
            {
                // Fallback: if humanMove provided but no human participant found
                player1Move = humanMove.Value;
                player2Move = GenerateRandomMove();
            }

            var gameResult = PlayGame(player1Move, player2Move);
            matchResult.Player1Move = player1Move;
            matchResult.Player2Move = player2Move;
            matchResult.Result = gameResult;  // Updated property name
        }        matchResult.IsComplete = true;
        return matchResult;
    }


}




    

