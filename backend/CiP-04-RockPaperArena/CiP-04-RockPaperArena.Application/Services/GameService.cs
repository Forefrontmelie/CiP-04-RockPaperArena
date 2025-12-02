using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;

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

        Console.WriteLine($"BEFORE: P1={humanMatch.player1Wins}, P2={humanMatch.player2Wins}, Draw={humanMatch.draw}");


        // mutate fields on humanMatch (example)
        if (!humanMatch.IsComplete)
        {
            var updated = PlayMatch(humanMatch, currentRound, move);
            matchesThisRound[0] = updated;
        }

        Console.WriteLine($"AFTER: P1={humanMatch.player1Wins}, P2={humanMatch.player2Wins}, Draw={humanMatch.draw}");

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

         // Process AI matches (1 to n-1)
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

            // Process AI matches (1 to n-1)
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







    public void PlayGame(Move player1Move, Move player2Move, Match match)
    {        
        if (player1Move == player2Move)
        { 
            match.draw++;
            return;
        }


        switch (player1Move)
        {
            case Move.Rock:
                if (player2Move == Move.Scissors)
                {
                    match.player1Wins++;
                    break;
                }
                else
                    match.player2Wins++;
                break;

            case Move.Paper:
                if (player2Move == Move.Rock)
                {
                    match.player1Wins++;
                    break;
                }
                else
                    match.player2Wins++;
                break;

            case Move.Scissors:
                if (player2Move == Move.Paper)
                {
                    match.player1Wins++;
                    break;
                }
                else
                    match.player2Wins++;
                break;

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



     public Match PlayMatch(Match match, int roundNumber, Move? humanMove) 
        {
            if (match.IsComplete)
                return match;

            if (match.currentRound == 1 && match.subRound == 0)
            {
                match.subRound++;
            }
            else
            {
                //match.currentRound++; 
                match.subRound++;
            }
            

            int GamesPlayed() => match.player1Wins + match.player2Wins + match.draw;
            bool ShouldComplete() => match.player1Wins == 2 || match.player2Wins == 2 || GamesPlayed() == 3;

            if (humanMove == null)
            {
                while(!ShouldComplete())
                {
                    if (GamesPlayed() >= 3) break;

                    // Both players are AI
                    var p1 = GenerateRandomMove();
                    var p2 = GenerateRandomMove();
                    PlayGame(p1, p2, match);
                }
                match.IsComplete = true;            
            }
            else
            {
                // Human vs AI: one game per call
                if (!ShouldComplete())
                { 
                    // Human vs AI match - human is always the fixed participant, hence player1
                    var p1 = humanMove.Value;
                    var p2 = GenerateRandomMove();   
                    PlayGame(p1, p2, match);            
                }

                if (ShouldComplete())
                    match.IsComplete = true;
            }                
        
            return match;
        }





    public Scoreboard UpdateScoreboard_OLD(Dictionary<int, IList<Match>> roundSchedule, Scoreboard currentScoreboard, int currentRound)
    {
        foreach (var match in roundSchedule[currentRound])
        {
            if (match.IsComplete)
            {
                // Update player 1 score
                var player1Scores = currentScoreboard.scores[match.Player1.Id];
                var lastScoreP1 = player1Scores.Last();

                var wins = lastScoreP1.Wins + match.player1Wins;
                var losses = lastScoreP1.Losses + match.player2Wins;
                var draws = lastScoreP1.Draws + match.draw;

                var newScoreP1 = new Score(lastScoreP1.Name, lastScoreP1.Id, currentRound)
                {
                    Wins = wins,
                    Losses = losses,
                    Draws = draws,
                    Points = CalculatePoints(wins, losses, draws)
                }; 
                player1Scores.Add(newScoreP1);

                // Update player 2 score
                var player2Scores = currentScoreboard.scores[match.Player2.Id];
                var lastScoreP2 = player2Scores.Last();

                wins = lastScoreP2.Wins + match.player2Wins;
                losses = lastScoreP2.Losses + match.player1Wins;
                draws = lastScoreP2.Draws + match.draw;

                var newScoreP2 = new Score(lastScoreP2.Name, lastScoreP2.Id, currentRound)
                {
                    Wins = lastScoreP2.Wins + match.player2Wins,
                    Losses = lastScoreP2.Losses + match.player1Wins,
                    Draws = lastScoreP2.Draws + match.draw,
                    Points = CalculatePoints(wins, losses, draws)
                };
                player2Scores.Add(newScoreP2);
            }
        }

        return currentScoreboard;

    }




    public Scoreboard UpdateScoreboard(Dictionary<int, IList<Match>> roundSchedule, Scoreboard currentScoreboard, int currentRound)
    {
        if (!roundSchedule.ContainsKey(currentRound))
            return currentScoreboard;

        foreach (var match in roundSchedule[currentRound])
        {
            // Only process completed matches
            if (!match.IsComplete)
            {
                Console.WriteLine($"Skipping incomplete match: {match.Player} vs {match.Opponent}");
                continue;
            }

            // Calculate points based on match outcome
            int player1Points = CalculatePoints(match.player1Wins, match.player2Wins, match.draw);
            int player2Points = CalculatePoints(match.player2Wins, match.player1Wins, match.draw);

            // Get current score list for each player
            var p1ScoreList = currentScoreboard.scores[match.Player1.Id];
            var p2ScoreList = currentScoreboard.scores[match.Player2.Id];

            // Get the latest (last) score entry
            var lastP1Score = p1ScoreList.Last();
            var lastP2Score = p2ScoreList.Last();

            // Create new score entries with cumulative values
            var newP1Score = new Score(lastP1Score.Name, lastP1Score.Id, currentRound)
            {
                Wins = lastP1Score.Wins + (match.player1Wins == 2 ? 1 : 0),
                Losses = lastP1Score.Losses + (match.player2Wins == 2 ? 1 : 0),
                Draws = lastP1Score.Draws + (match.player1Wins < 2 && match.player2Wins < 2 ? 1 : 0),
                Points = lastP1Score.Points + player1Points
            };

            var newP2Score = new Score(lastP2Score.Name, lastP2Score.Id, currentRound)
            {
                Wins = lastP2Score.Wins + (match.player2Wins == 2 ? 1 : 0),
                Losses = lastP2Score.Losses + (match.player1Wins == 2 ? 1 : 0),
                Draws = lastP2Score.Draws + (match.player1Wins < 2 && match.player2Wins < 2 ? 1 : 0),
                Points = lastP2Score.Points + player2Points
            };

            // Add new score entries to the history list
            p1ScoreList.Add(newP1Score);
            p2ScoreList.Add(newP2Score);

            Console.WriteLine($"Updated scores - {match.Player}: +{player1Points}pts (total: {newP1Score.Points}), {match.Opponent}: +{player2Points}pts (total: {newP2Score.Points})");
        }

        return currentScoreboard;
    }




    public int CalculatePoints(int wins, int losses, int draws)
    {
        // Match outcome (best-of-3):
        // - Win = 3 points
        // - Draw = 1 point
        // - Loss = 0 points

        if (wins == 2)
            return 3;  // Won the match

        if (losses == 2)
            return 0;  // Lost the match

        // Neither player reached 2 wins after 3 games = draw
        return 1;
    }

}




    

 
