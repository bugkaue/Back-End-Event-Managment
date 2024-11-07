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
public class ParticipanteController : ControllerBase
{
    private readonly IParticipanteService _service;

    public ParticipanteController(IParticipanteService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ReadParticipanteDto>> CreateParticipante(CreateParticipanteDto createParticipanteDto)
    {
        var participante = await _service.CreateAsync(createParticipanteDto);
        return CreatedAtAction(nameof(GetParticipanteById), new { id = participante.Id }, participante);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadParticipanteDto>> GetParticipanteById(int id)
    {
        var participante = await _service.GetByIdAsync(id);
        return participante == null ? NotFound() : Ok(participante);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadParticipanteDto>>> GetAllParticipantes()
    {
        var participantes = await _service.GetAllAsync();
        return Ok(participantes);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipante(int id, UpdateParticipanteDto updateParticipanteDto)
    {
        if (id != updateParticipanteDto.Id)
        {
            return BadRequest();
        }

        await _service.UpdateAsync(id, updateParticipanteDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipante(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetParticipanteCount()
    {
        var count = await _service.CountAsync();
        return Ok(count);
    }
}