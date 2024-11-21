using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipanteController(IParticipanteService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReadParticipanteDto>> CreateParticipante(CreateParticipanteDto createParticipanteDto)
    {
        var participante = await service.CreateAsync(createParticipanteDto);
        return CreatedAtAction(nameof(GetParticipanteById), new { id = participante.Id }, participante);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadParticipanteDto>> GetParticipanteById(int id)
    {
        var participante = await service.GetByIdAsync(id);
        return participante == null ? NotFound() : Ok(participante);
    }

    [Authorize(Roles = "Admin" )]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadParticipanteDto>>> GetAllParticipantes()
    {
        var participantes = await service.GetAllAsync();
        var participantesComEventos = new List<ReadParticipanteDto>();

        foreach (var participante in participantes)
        {
            participante.NumeroEventosInscritos = await service.GetNumeroEventosInscritosAsync(participante.Id);
            participantesComEventos.Add(participante);
        }

        return Ok(participantesComEventos);
    }
    

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipante(int id, UpdateParticipanteDto updateParticipanteDto)
    {
        if (id != updateParticipanteDto.Id)
        {
            return BadRequest();
        }

        await service.UpdateAsync(id, updateParticipanteDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipante(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin" )]
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetParticipanteCount()
    {
        var count = await service.CountAsync();
        return Ok(count);
    }
}