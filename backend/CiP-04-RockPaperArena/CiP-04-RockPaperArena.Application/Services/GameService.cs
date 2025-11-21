using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using System.Linq.Expressions;

namespace CiP_04_RockPaperArena.Application.Services;

public class GameService : IGameService
{

    public GameService()
    {
    }


    public Dictionary<int, IList<Match>> PlayHumanMatchCurrentRound(Dictionary<int, IList<Match>> RoundSchedule, int currentRound, Move move)
    {
        Dictionary<int, IList<Match>> updatedSchedule = new Dictionary<int, IList<Match>>();

        var matchesThisRound = RoundSchedule[currentRound];
        var humanMatch = matchesThisRound[0];  // reference to existing object

        // mutate fields on humanMatch (example)
        if (!humanMatch.IsComplete)
        {
            var updated = PlayMatch(humanMatch, currentRound, move);
            // If PlayMatch mutates and returns same instance, no need to reassign.
            // If it returns a different Match object, then:
            matchesThisRound[0] = updated;
        }

        return RoundSchedule; // unchanged structure, mutated contents
    }


    public Dictionary<int, IList<Match>> PlayAiMatchesCurrentRound(Dictionary<int, IList<Match>> RoundSchedule, int currentRound)
    {
        Dictionary<int, IList<Match>> updatedSchedule = new Dictionary<int, IList<Match>>();

        var matchesThisRound = RoundSchedule[currentRound];
        var updatedMatches = new List<Match>();

         // Add the human match (index 0) unchanged
         if (matchesThisRound.Count > 0)
         {
            updatedMatches.Add(matchesThisRound[0]);
         }

         // Process AI matches (indexes 1 to n-1)
         for (int i = 1; i < matchesThisRound.Count; i++)
         {
            var match = matchesThisRound[i];
            var updatedMatch = PlayMatch(match, currentRound, null);
            updatedMatches.Add(updatedMatch);
         }

         updatedSchedule[currentRound] = updatedMatches;

        return updatedSchedule;
    }



    public Dictionary<int, IList<Match>> PlayAllAiMatches(Dictionary<int, IList<Match>> RoundSchedule)
    {
        Dictionary<int, IList<Match>> updatedSchedule = new Dictionary<int, IList<Match>>();

        foreach (var roundEntry in RoundSchedule)
        {
            var roundNumber = roundEntry.Key;
            var matches = roundEntry.Value;
            var updatedMatches = new List<Match>();

            // Add the human match (index 0) unchanged
            if (matches.Count > 0)
            {
                updatedMatches.Add(matches[0]);
            }

            // Process AI matches (indexes 1 to n-1)
            for (int i = 1; i < matches.Count; i++)
            {
                var match = matches[i];
                var updatedMatch = PlayMatch(match, roundNumber, null);
                updatedMatches.Add(updatedMatch);
            }

            updatedSchedule[roundNumber] = updatedMatches;
        }


        return updatedSchedule;
    }







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



 public Match PlayMatch(Match match, int roundNumber, Move? humanMove)   //   <<<---------------- ÄNDRA SÅ HANTERAR BEST OF 3 - och sparar detta i MatchResult!
    {             
        Move player1Move;
        Move player2Move;

        if (humanMove == null)
        {
            while(match.player1Wins < 2 && match.player2Wins < 2 && match.draw < 2)
            {
                // Both players are AI
                player1Move = GenerateRandomMove();
                player2Move = GenerateRandomMove();
                var gameResult = PlayGame(player1Move, player2Move);

                CheckWinner(gameResult, match);  
            }
            match.IsComplete = true;            
        }
        else
        {
            // Human vs AI match - human is always the fixed participant, hence player1
            player1Move = humanMove.Value;
            player2Move = GenerateRandomMove();   
            var gameResult = PlayGame(player1Move, player2Move);

            CheckWinner(gameResult, match);

            if (!(match.player1Wins < 2) && !(match.player2Wins < 2) && !(match.draw < 2))
               match.IsComplete = true;
        }                
        
        return match;
    }


 public void CheckWinner(GameResult gameResult, Match match)
    {
        switch (gameResult)
        {
            case GameResult.P1:
                match.player1Wins++;
                match.currentRound++;
                break;
            case GameResult.P2:
                match.player2Wins++;
                match.currentRound++;
                break;
            case GameResult.Draw:
                match.draw++;
                match.currentRound++;
                break;
        }
    }


/*
    public Match PlayMatch(Participant player1, Participant player2, int roundNumber, Move? humanMove = null)   //   <<<---------------- ÄNDRA SÅ HANTERAR BEST OF 3 - och sparar detta i MatchResult!
    {
        var matchResult = new Match(player1, player2, roundNumber);

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
*/

}




    

 
