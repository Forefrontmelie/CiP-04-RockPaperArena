using CiP_04_RockPaperArena.Application.Services;
using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CiP_04_RockPaperArena.Api.Controllers; 

[ApiController]
[Route("[controller]")]
public class ApiController(ITournamentService tournament, IParticipantRepository participant) : ControllerBase
{
    // POST /tournament/start
    [HttpPost("tournament/start")]
    public async Task<IActionResult> StartTournament([FromBody] StartTournamentDto dto)
    {
        try
        {
            await tournament.StartTournamentAsync(dto.name, dto.players);

            var status = await tournament.GetHumanPlayersCurrentGameStatusAsync();
            var scoreboard = await tournament.GetScoreboardAsync();

            return Ok(new { 
                message = "Tournament started successfully", 
                scoreboard,
                playerName = dto.name, 
                opponent = status.Opponent,
                totalPlayers = dto.players,
                currentRound = status.CurrentRound,
                totalRounds = tournament.GetMaxNumberOfRounds(dto.players)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /tournament/status
    [HttpGet("tournament/status")]
    public async Task<IActionResult> GetTournamentStatus()
    {
        try
        {
            var currentTournament = await tournament.GetCurrentTournamentAsync();
            if (currentTournament == null || !tournament.HasActiveTournament)
            {
                return Ok(new { message = "No active tournament" });
            }
                
            //var response = await tournament.GetHumanPlayersCurrentGameStatusAsync();
            var scoreboard = await tournament.GetScoreboardAsync();

            return Ok(new
            {
                //response,
                scoreboard
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /tournament/play
    [HttpPost("tournament/play")]
    public async Task<IActionResult> PlayMove([FromBody] int move)
    {
        try
        {
            await tournament.PlayMoveAsync(move);
            var response = await tournament.GetHumanPlayersCurrentGameStatusAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /tournament/advance
    [HttpPost("tournament/advance")]
    public async Task<IActionResult> AdvanceTournament()
    {
        try
        {
            var currentTournament = await tournament.GetCurrentTournamentAsync();
            if (currentTournament == null)
                return BadRequest(new { error = "No active tournament" });
            
            var roundBeforeAdvance = currentTournament.CurrentRound;
            
            await tournament.AdvanceRoundAsync();

            // Reload after advance
            currentTournament = await tournament.GetCurrentTournamentAsync();
            if (currentTournament == null)
                return BadRequest(new { error = "Tournament not found after advance" });
            
            var justCompletedFinalRound = roundBeforeAdvance == currentTournament.TotalRounds 
                                          && currentTournament.CurrentRound == currentTournament.TotalRounds;

            var response = new
            {
                message = justCompletedFinalRound
                    ? "Final round complete. Call /tournament/final to finish tournament."
                    : $"Round {roundBeforeAdvance} complete. Advanced to round {currentTournament.CurrentRound}.",
                //currentRound = currentTournament.CurrentRound,
                //totalRounds = currentTournament.TotalRounds,
                isComplete = justCompletedFinalRound
                   ? "true"
                   : "false",
                scoreboard = await tournament.GetScoreboardAsync()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET /tournament/final
    [HttpGet("tournament/final")]
    public async Task<IActionResult> GetFinalResult()
    {
        try
        {
            await tournament.FinishTournamentAsync();

            var scoreboard = await tournament.GetScoreboardAsync();

            var sortedScores = scoreboard.scores.Values
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.Wins)
                .ToList();

            var highestPoints = sortedScores.First().Points;
            var highestWins = sortedScores.First().Wins;

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






    // GET /participants
    [HttpGet("participants")]
    public async Task<IActionResult> GetAllParticipants()
    {
        try
        {
            var participants = await participant.GetAllParticipantsAsync();
            return Ok(participants);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST /player
    [HttpPost("player")]
    public async Task<IActionResult> AddPlayer([FromBody] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Player name cannot be empty" });
            }
            
            await participant.AddParticipantAsync(name);
            return Ok(new { message = "Player added successfully", playerName = name });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE /player/:id
    [HttpDelete("player/{id}")]
    public async Task<IActionResult> RemovePlayer(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid player ID" });
            }
            
            await participant.RemoveParticipantAsync(id);
            return Ok(new { message = "Player removed successfully", playerId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}