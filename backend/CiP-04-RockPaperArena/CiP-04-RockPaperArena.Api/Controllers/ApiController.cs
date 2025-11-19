using Microsoft.AspNetCore.Mvc;
using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;

namespace CiP_04_RockPaperArena.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController(IGameService gameService, IParticipantRepository participant) : ControllerBase
{

    [HttpGet("doesitwork")]
    public string workGodDammit()
    {
        return "Yay!";
    }



    // ::::::::::::::::::::::::: PARTICIPANT ROUTES ::::::::::::::::::::::::: //

    //GET		/participants/				Returnerar alla participants.
    [HttpGet("participants")]
    public IList<Participant> GetAllParticipants()
    {
        return participant.GetAllParticipants();
    }



    //POST	/player	(Bonus) 		Lägg till en ny deltagare i listan.
    [HttpPost("player")]
    public void AddPlayer([FromBody] string name)
    {
        participant.AddParticipant(name);
    }

    //DELETE	/player/:id	(Bonus) 	Ta bort en deltagare ur listan baserat på ID.
    [HttpDelete("player/{id}")]
    public void RemovePlayer(int id)
    {
        participant.RemoveParticipant(id);
    }

}
