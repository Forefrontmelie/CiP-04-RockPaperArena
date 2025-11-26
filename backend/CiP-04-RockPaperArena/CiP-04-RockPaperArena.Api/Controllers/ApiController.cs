using CiP_04_RockPaperArena.Application.Services;
using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CiP_04_RockPaperArena.Api.Controllers; // ------------------ !!!!!! Lägg till: Task<IActionResult> och async och await !!!!!! ------------------ //

[ApiController]
[Route("[controller]")]
public class ApiController(ITournamentService tournament, IParticipantRepository participant) : ControllerBase
{

    
    // POST	/tournament/start Startar ny turnering.Skapar par f�r runda 1 baserat p� din round-robin-funktion.Body: { "name": "Alice", "players": 8 }.
    [HttpPost("tournament/start")]
    public IActionResult StartTournament([FromBody] StartTournamentDto dto)
    {
        try
        {
            tournament.StartTournament(dto.name, dto.players);

            var currentRound = tournament.GetCurrentRoundNumber();
            
            return Ok(new { 
                message = "Tournament started successfully", 
                playerName = dto.name, 
                totalPlayers = dto.players,
                currentRound = currentRound,
                totalRounds = tournament.GetMaxNumberOfRounds(dto.players),
               // pairs = currentRound?.Pairs
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET	    /tournament/status	
    // Returnerar aktuell runda, din n�sta motst�ndare och scoreboard, samt information om delrundor i matchen, t.ex. "round": 1 of 3, "playerWins": 1, "opponentWins": 0.
    // �ven status f�r om �vriga matcher i rundan �r f�rdigspelade (b�st av 3) kan ing�.
    [HttpGet("tournament/status")]
    public IActionResult GetTournamentStatus()
    {
        try
        {
            var currentTournament = tournament.GetCurrentTournament();
            if (currentTournament == null || !tournament.HasActiveTournament)
            {
                return Ok(new { message = "No active tournament" });
            }
                
            var response = tournament.GetHumanPlayersCurrentGameStatus();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }



    // POST	/tournament/play	Spelar ett drag i den aktuella matchen f�r din deltagare.
    // Body: { "move": 1 } (0 = Rock, 1 = Paper, 2 = Scissors).
    // Returnerar resultatet av draget, din motst�ndares drag och aktuell score i matchen.
    [HttpPost("tournament/play")]
    public IActionResult PlayMove([FromBody] int move)
    {
        try
        {
            tournament.PlayMove(move);
            var response = tournament.GetHumanPlayersCurrentGameStatus();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    // POST	/tournament/advance
    [HttpPost("tournament/advance")]
    public IActionResult AdvanceTournament()
    {
        try
        {
        var currentTournament = tournament.GetCurrentTournament();
        var roundBeforeAdvance = currentTournament.CurrentRound;
        
        tournament.AdvanceRound();

        // Reload after advance
        currentTournament = tournament.GetCurrentTournament();
        
        // Check if we just completed the final round
        // (CurrentRound didn't increment because we were already at TotalRounds)
        var justCompletedFinalRound = roundBeforeAdvance == currentTournament.TotalRounds 
                                      && currentTournament.CurrentRound == currentTournament.TotalRounds;

        var response = new
        {
            message = justCompletedFinalRound
                ? "Final round complete. Call /tournament/final to finish tournament."
                : $"Round {roundBeforeAdvance} complete. Advanced to round {currentTournament.CurrentRound}.",
            currentRound = currentTournament.CurrentRound,
            totalRounds = currentTournament.TotalRounds,
            scoreboard = tournament.GetScoreboard()
        };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    // GET	    /tournament/final	Returnerar slutresultatet och vinnare när alla rundor är spelade.
    [HttpGet("tournament/final")]
    public IActionResult GetFinalResult()
    {
        try
        {
            // This will throw if tournament isn't ready to finish
            tournament.FinishTournament();

            var scoreboard = tournament.GetScoreboard();

        // Sort by points, then by wins
        var sortedScores = scoreboard.scores.Values
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.Wins)
            .ToList();

        // Get the highest score
        var highestPoints = sortedScores.First().Points;
        var highestWins = sortedScores.First().Wins;

        // Find all participants with the same highest score and wins (handling ties)
        var winners = sortedScores
            .Where(s => s.Points == highestPoints && s.Wins == highestWins)
            .Select(s => s.Name)
            .ToList();

        var isTie = winners.Count > 1;

            return Ok(new
            {
                message = isTie 
                    ? $"Tournament finished with a {winners.Count}-way tie!" 
                    : "Tournament finished",
                winners = winners,
                isTie = isTie,
                finalScoreboard = scoreboard
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }









    // ::::::::::::::::::::::::: PARTICIPANT ROUTES ::::::::::::::::::::::::: //

    //GET		/participants/				Returnerar alla participants.
    [HttpGet("participants")]
    public IActionResult GetAllParticipants()
    {
        try
        {
            var participants = participant.GetAllParticipants();
            return Ok(participants);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }



    //POST	/player	(Bonus) 		L�gg till en ny deltagare i listan.
    [HttpPost("player")]
    public IActionResult AddPlayer([FromBody] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Player name cannot be empty" });
            }
            
            participant.AddParticipant(name);
            return Ok(new { message = "Player added successfully", playerName = name });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //DELETE	/player/:id	(Bonus) 	Ta bort en deltagare ur listan baserat p� ID.
    [HttpDelete("player/{id}")]
    public IActionResult RemovePlayer(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid player ID" });
            }
            
            participant.RemoveParticipant(id);
            return Ok(new { message = "Player removed successfully", playerId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}
