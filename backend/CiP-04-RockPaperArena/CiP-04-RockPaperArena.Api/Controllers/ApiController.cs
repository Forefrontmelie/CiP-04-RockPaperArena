using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CiP_04_RockPaperArena.Api.Controllers;

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
            return Ok(new { message = "Tournament started successfully", playerName = dto.name, totalPlayers = dto.players });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    
        /*
    // POST	/tournament/play Du g�r ditt drag(rock, paper, scissors). Backend avg�r resultatet f�r delrundan, uppdaterar po�ng och sparar resultatet f�r denna delrunda i matchen.Returnerar status f�r matchens delrundor och aktuell st�llning.
    [HttpPost("tournament/play")]
    public MatchStatusDTO PlayRound([FromBody] PlayerMoveDTO dto)
    {
        return tournament.PlayRound(dto.Move);
    }


    // POST    /tournament/advance
    // Simulerar alla �vriga/automatiska matcher i den p�g�ende rundan som b�st av 3 (tills n�gon har 2 delrundevinster),
    // uppdaterar scoreboard och s�tter upp n�sta runda via round-robin-logiken f�rst n�r samtliga matcher i rundan �r f�rdigspelade.
    [HttpPost("tournament/advance")]
    public void AdvanceTournament()
    {
        tournament.AdvanceTournament();
    }
    */


    
    
    // GET	    /tournament/status	
    // Returnerar aktuell runda, din n�sta motst�ndare och scoreboard, samt information om delrundor i matchen, t.ex. "round": 1 of 3, "playerWins": 1, "opponentWins": 0.
    // �ven status f�r om �vriga matcher i rundan �r f�rdigspelade (b�st av 3) kan ing�.
    [HttpGet("tournament/status")]
    public IActionResult GetTournamentStatus()
    {
        try
        {
            // TODO: Implement actual tournament status logic
            return Ok(new { message = "Tournament status retrieved", status = "active" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    // GET	    /tournament/final	Returnerar slutresultatet och vinnare n�r alla rundor �r spelade.
    [HttpGet("tournament/final")]
    public IActionResult GetFinalResult()
    {
        try
        {
            // TODO: Implement actual final result logic
            return Ok(new { message = "Final result retrieved", winner = "TBD", results = new object[] { } });
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
